using Circles.Command.Application.Commands;
using Circles.Command.Application.EventProducer;
using Circles.Domain.Entities;
using Circles.Domain.Repositories;
using Core.DTOs;
using Core.Events.PublicEvents;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;

namespace Circles.Command.Application.Handlers
{
    public class CreateCircleCommandHandler : IMessageHandler<CreateCircleCommand>
    {
        private readonly CirclesRepository _circlesRepository;
        private readonly UserCircleRepository _userCircleRepository;
        private readonly JoinRequestRepository _requestRepository;
        private readonly JoinCircleEventProducer _eventProducer;
        private readonly ILogger<CreateCircleCommandHandler> _logger;

        public CreateCircleCommandHandler(CirclesRepository circlesRepository,
                                          UserCircleRepository userCircleRepository,
                                          JoinRequestRepository requestRepository,
                                          JoinCircleEventProducer eventProducer,
                                          ILogger<CreateCircleCommandHandler> logger)
        {
            _circlesRepository = circlesRepository;
            _userCircleRepository = userCircleRepository;
            _requestRepository = requestRepository;
            _eventProducer = eventProducer;
            _logger = logger;
        }

        public async Task<BaseResponse> HandleAsync(CreateCircleCommand command)
        {
            command.Users ??= new List<Guid>();
            command.Users = command.Users.Distinct().ToList();

            var circle = await _circlesRepository.CheckExistsForUser(command.Name, command.CreatorId);
            if (circle != null) return new BaseResponse { ResponseCode = 409, Message = $"You already have a circle named '{command.Name}'" };

            using var session = await _circlesRepository.GetSession();
            try
            {
                session.StartTransaction();
                await _circlesRepository.SaveAsync(
                    new CircleModel
                    {
                        CircleId = command.CircleId,
                        CreatorId = command.CreatorId,
                        Name = command.Name,
                        Color = command.Color
                    }
                );

                await _userCircleRepository.SaveAsync(
                    new UserCircleModel
                    {
                        CircleId = command.CircleId,
                        UserId = command.CreatorId
                    }
                );

                if (command.Users.Contains(command.CreatorId))
                {
                    command.Users.Remove(command.CreatorId);
                }

                var insertTask = command.Users.Select(
                    uId => Task.Run(() => _requestRepository.SaveAsync(
                            new JoinRequestModel
                            {
                                UserId = uId,
                                CircleId = command.CircleId,
                                InviterId = command.CreatorId
                            }
                        )
                    )
                );

                await Task.WhenAll(insertTask);

                var produceTask = command.Users.Select(
                    uId => Task.Run(() => _eventProducer.ProduceAsync(
                            new JoinCircleRequestPublicEvent
                            (
                                command.CircleId,
                                uId,
                                command.CreatorId
                            )
                        )
                    )
                );

                await Task.WhenAll(produceTask);
                session.CommitTransaction();

                var created = await _circlesRepository.GetUsersInCircle(command.CircleId);
                return new BaseResponse { ResponseCode = 201, Data = created };
            }
            catch (Exception e)
            {
                session.AbortTransaction();
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return new BaseResponse { ResponseCode = 500, Message = "Something went wrong, please try again later." };
            }
        }

        public async Task<BaseResponse> HandleAsync(BaseMessage command)
        {
            return await HandleAsync((CreateCircleCommand)command);
        }
    }
}