using User.Common.Events;
using User.Query.Domain.Repositories;

namespace User.Query.Application.Handlers
{
    public class EmailVerifiedEventHandler
    {
        private readonly UserRepository _userRepository;
        public EmailVerifiedEventHandler(IServiceProvider serviceProvider, UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task HandleAsync(EmailVerifiedEvent xEvent)
        {
            await _userRepository.VerifyEmail(xEvent);
        }
    }
}