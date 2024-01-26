using Core.MessageHandling;
using User.Common.Events;
using User.Query.Domain.Repositories;

namespace User.Query.Application.Handlers
{
    public class PasswordUpdatedEventHandler : IEventHandler<PasswordUpdatedEvent>
    {
        private readonly UserRepository _userRepository;
        public PasswordUpdatedEventHandler(IServiceProvider serviceProvider, UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task HandleAsync(PasswordUpdatedEvent xEvent)
        {
            await _userRepository.UpdateUsersPassword(xEvent);
        }
    }
}