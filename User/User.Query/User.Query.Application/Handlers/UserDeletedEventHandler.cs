using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;
using User.Common.Events;
using User.Query.Domain.Repositories;

namespace User.Query.Application.Handlers
{
    public class UserDeletedEventHandler : IMessageHandler<UserDeletedEvent>
    {
        private readonly UserRepository _userRepository;
        private readonly RefreshTokenRepository _refreshTokenRepository;
        private readonly ILogger<UserDeletedEventHandler> _logger;
        public UserDeletedEventHandler(UserRepository userRepository,
                                       ILogger<UserDeletedEventHandler> logger,
                                       RefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _logger = logger;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<BaseResponse> HandleAsync(UserDeletedEvent xEvent)
        {
            try
            {
                await _userRepository.DeleteUser(xEvent);
                await _refreshTokenRepository.DeleteRefreshTokenByUserId(xEvent.Id.ToString());
                return new BaseResponse { ResponseCode = 200, Message = "User deleted successfully." };
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return new BaseResponse { ResponseCode = 500, Message = "Something went wrong, please try again later." };
            }
        }

        public async Task<BaseResponse> HandleAsync(BaseMessage xEvent)
        {
            return await HandleAsync((UserDeletedEvent)xEvent);
        }
    }
}