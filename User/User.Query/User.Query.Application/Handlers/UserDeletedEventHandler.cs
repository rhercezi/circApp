using Core.MessageHandling;
using Core.Messages;
using User.Common.Events;
using User.Query.Domain.Repositories;

namespace User.Query.Application.Handlers
{
    public class UserDeletedEventHandler : IEventHandler<UserDeletedEvent>
    {
        private readonly UserRepository _userRepository;
        public UserDeletedEventHandler(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task HandleAsync(UserDeletedEvent xEvent)
        {
            await _userRepository.DeleteUser(xEvent);
        }

        public async Task HandleAsync(BaseEvent xEvent)
        {
            await HandleAsync((UserDeletedEvent)xEvent);
        }
    }
}