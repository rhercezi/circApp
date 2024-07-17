using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using User.Common.Events;
using User.Query.Domain.Repositories;

namespace User.Query.Application.Handlers
{
    public class EmailVerifiedEventHandler : IMessageHandler<EmailVerifiedEvent>
    {
        private readonly UserRepository _userRepository;
        public EmailVerifiedEventHandler(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<BaseResponse> HandleAsync(EmailVerifiedEvent xEvent)
        {
            try
            {
                await _userRepository.VerifyEmail(xEvent);
                return new BaseResponse { ResponseCode = 200, Message = "Email verified successfully." };
            }
            catch (Exception e) 
            {
                return new BaseResponse { ResponseCode = 500, Message = e.Message };
            }
        }

        public async Task<BaseResponse> HandleAsync(BaseMessage xEvent)
        {
            return await HandleAsync((EmailVerifiedEvent)xEvent);
        }
    }
}