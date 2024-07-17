using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;
using User.Common.Events;
using User.Query.Domain.Repositories;

namespace User.Query.Application.Handlers
{
    public class UserUpdatedEventHandler : IMessageHandler<UserEditedEvent>
    {
        private readonly UserRepository _userRepository;
        private readonly ILogger<UserUpdatedEventHandler> _logger;
        public UserUpdatedEventHandler(UserRepository userRepository, ILogger<UserUpdatedEventHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<BaseResponse> HandleAsync(UserEditedEvent xEvent)
        {
            try
            {
                await _userRepository.UpdateUser(xEvent);
                return new BaseResponse { ResponseCode = 200, Message = "User updated successfully." };
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return new BaseResponse { ResponseCode = 500, Message = "Something went wrong, please try again later." };
            }
        }

        public async Task<BaseResponse> HandleAsync(BaseMessage xEvent)
        {
            return await HandleAsync((UserEditedEvent)xEvent);
        }
    }
}