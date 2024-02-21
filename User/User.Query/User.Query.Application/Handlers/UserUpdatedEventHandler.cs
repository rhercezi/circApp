using Core.MessageHandling;
using Core.Messages;
using User.Common.Events;
using User.Query.Domain.Repositories;

namespace User.Query.Application.Handlers
{
    public class UserUpdatedEventHandler : IEventHandler<UserEditedEvent>
    {
        private readonly UserRepository _userRepository;
        public UserUpdatedEventHandler(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task HandleAsync(UserEditedEvent xEvent)
        {
            await _userRepository.UpdateUser(xEvent);
        }

        public async Task HandleAsync(BaseEvent xEvent)
        {
            await HandleAsync((UserEditedEvent)xEvent);
        }
    }
}