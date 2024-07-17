using Circles.Domain.Repositories;
using Core.DTOs;
using Core.Events.PublicEvents;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;

namespace Circles.Command.Application.Handlers
{
    public class UserDeletedEventHandler : IMessageHandler<UserDeletedPublicEvent>
    {
        private readonly UserRepository _userRepository;
        private readonly UserCircleRepository _userCircleRepository;
        private readonly ILogger<UserDeletedEventHandler> _logger;

        public UserDeletedEventHandler(UserRepository userRepository,
                                       UserCircleRepository userCircleRepository,
                                       ILogger<UserDeletedEventHandler> logger)
        {
            _userRepository = userRepository;
            _userCircleRepository = userCircleRepository;
            _logger = logger;
        }

        public async Task<BaseResponse> HandleAsync(UserDeletedPublicEvent xEvent)
        {
            using var session = await _userRepository.GetSession();
            try
            {
                session.StartTransaction();
                await _userRepository.DeleteUser(xEvent.Id);
                await _userCircleRepository.DeleteByUser(xEvent.Id);
                session.CommitTransaction();

                return new BaseResponse { ResponseCode = 200 };
            }
            catch (Exception e)
            {
                session.AbortTransaction();
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return new BaseResponse { ResponseCode = 500 };
            }
        }

        public async Task<BaseResponse> HandleAsync(BaseMessage xEvent)
        {
            return await HandleAsync((UserDeletedPublicEvent)xEvent);
        }
    }
}