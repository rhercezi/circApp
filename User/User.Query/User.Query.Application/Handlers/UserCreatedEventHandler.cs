using Core.MessageHandling;
using User.Common.Events;
using User.Query.Domain.Repositories;

namespace User.Query.Application.Handlers
{
    public class UserCreatedEventHandler : IEventHandler<UserCreatedEvent>
    {
        private readonly UserRepository _userRepository;
        public UserCreatedEventHandler(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task HandleAsync(UserCreatedEvent xEvent)
        {
            await _userRepository.CreateUser(xEvent);
        }
    }
}