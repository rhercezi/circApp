using Circles.Command.Application.Commands;
using Circles.Command.Application.EventProducer;
using Circles.Domain.Entities;
using Circles.Domain.Repositories;
using Core.Events.PublicEvents;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;

namespace Circles.Command.Application.Handlers
{
    public class CreateCircleCommandHandler : ICommandHandler<CreateCircleCommand>
    {
        public readonly CirclesRepository _circlesRepository;
        public readonly UserCircleRepository _userCircleRepository;
        public readonly JoinRequestRepository _requestRepository;
        public readonly JoinCircleEventProducer _eventProducer;
        public readonly ILogger<CreateCircleCommandHandler> _logger;

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

        public async Task HandleAsync(CreateCircleCommand command)
        {
            command.Users = command.Users.Distinct().ToList();
            using var session = await _circlesRepository.GetSession();
            try
            {
                session.StartTransaction();
                await _circlesRepository.SaveAsync(
                new CircleModel
                {
                    CircleId = command.CircleId,
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
            }
            catch (Exception e)
            {
                session.AbortTransaction();
                _logger.LogError($"{e.Message}\n{e.StackTrace}");
                throw;
            }
        }

        public async Task HandleAsync(BaseCommand command)
        {
            await HandleAsync((CreateCircleCommand)command);
        }
    }
}