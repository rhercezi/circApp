using Core.MessageHandling;
using Core.Messages;
using User.Common.Events;
using User.Query.Domain.Repositories;

namespace User.Query.Application.Handlers
{
    public class PasswordUpdatedEventHandler : IEventHandler<PasswordUpdatedEvent>
    {
        private readonly UserRepository _userRepository;
        public PasswordUpdatedEventHandler(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task HandleAsync(PasswordUpdatedEvent xEvent)
        {
            await _userRepository.UpdateUsersPassword(xEvent);
        }

        public async Task HandleAsync(BaseEvent xEvent)
        {
            await HandleAsync((PasswordUpdatedEvent)xEvent);
        }
    }
}