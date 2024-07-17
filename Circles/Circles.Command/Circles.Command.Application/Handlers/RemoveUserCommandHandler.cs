using Circles.Command.Application.Commands;
using Circles.Domain.Repositories;
using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;

namespace Circles.Command.Application.Handlers
{
    public class RemoveUserCommandHandler : IMessageHandler<RemoveUsersCommand>
    {
        private readonly UserCircleRepository _userCircleRepository;
        private readonly CirclesRepository _circlesRepository;
        private readonly ILogger<RemoveUserCommandHandler> _logger;
        public RemoveUserCommandHandler(UserCircleRepository userCircleRepository,
                                        CirclesRepository circlesRepository,
                                        ILogger<RemoveUserCommandHandler> logger)
        {
            _userCircleRepository = userCircleRepository;
            _circlesRepository = circlesRepository;
            _logger = logger;
        }

        public async Task<BaseResponse> HandleAsync(RemoveUsersCommand command)
        {
            var circle = await _circlesRepository.GetByIdAsync(command.CircleId);
            if (circle == null) return new BaseResponse { ResponseCode = 404, Message = "Circle not found" };

            using var session = await _userCircleRepository.GetSession();
            try
            {
                session.StartTransaction();

                var users = command.Users.Where(uid => uid != circle.CreatorId).ToList();
                var deleteTask = users.Select(
                    uId => Task.Run(() => _userCircleRepository.DeleteByUserAndCircle(uId, command.CircleId))
                );
                await Task.WhenAll(deleteTask);

                session.CommitTransaction();
                var newCircleUsers = await _circlesRepository.GetUsersInCircle(command.CircleId);
                return new BaseResponse { ResponseCode = 200, Data = newCircleUsers };
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return new BaseResponse { ResponseCode = 500, Message = "Something went wrong, please try again later." };
            }

        }

        public async Task<BaseResponse> HandleAsync(BaseMessage command)
        {
            return await HandleAsync((RemoveUsersCommand)command);
        }
    }
}