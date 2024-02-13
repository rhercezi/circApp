using Core.MessageHandling;
using User.Common.Events;
using User.Query.Domain.Repositories;

namespace User.Query.Application.Handlers
{
    public class UserCreatedEventHandler : IEventHandler<UserCreatedEvent>
    {
        private readonly UserRepository _userRepository;
        private readonly IServiceProvider _serviceProvider;
        public UserCreatedEventHandler(IServiceProvider serviceProvider, UserRepository userRepository)
        {
            _serviceProvider = serviceProvider;
            _userRepository = userRepository;
        }

        public async Task HandleAsync(UserCreatedEvent xEvent)
        {
            await _userRepository.CreateUser(xEvent);
        }
    }
}