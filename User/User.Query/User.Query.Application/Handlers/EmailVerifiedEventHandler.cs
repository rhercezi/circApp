using Core.MessageHandling;
using Core.Messages;
using User.Common.Events;
using User.Query.Domain.Repositories;

namespace User.Query.Application.Handlers
{
    public class EmailVerifiedEventHandler : IEventHandler<EmailVerifiedEvent>
    {
        private readonly UserRepository _userRepository;
        public EmailVerifiedEventHandler(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task HandleAsync(EmailVerifiedEvent xEvent)
        {
            await _userRepository.VerifyEmail(xEvent);
        }

        public async Task HandleAsync(BaseEvent xEvent)
        {
            await HandleAsync((EmailVerifiedEvent)xEvent);
        }
    }
}