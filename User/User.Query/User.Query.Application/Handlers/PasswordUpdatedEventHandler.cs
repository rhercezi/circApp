using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using User.Common.Events;
using User.Query.Domain.Repositories;

namespace User.Query.Application.Handlers
{
    public class PasswordUpdatedEventHandler : IMessageHandler<PasswordUpdatedEvent>
    {
        private readonly UserRepository _userRepository;
        public PasswordUpdatedEventHandler(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<BaseResponse> HandleAsync(PasswordUpdatedEvent xEvent)
        {
            await _userRepository.UpdateUsersPassword(xEvent);
            return new BaseResponse { ResponseCode = 200, Message = "Password updated successfully." };
        }

        public async Task<BaseResponse> HandleAsync(BaseMessage xEvent)
        {
            return await HandleAsync((PasswordUpdatedEvent)xEvent);
        }
    }
}