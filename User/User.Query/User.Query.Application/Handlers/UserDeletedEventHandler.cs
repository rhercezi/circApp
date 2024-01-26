using Core.MessageHandling;
using User.Common.Events;
using User.Query.Domain.Repositories;

namespace User.Query.Application.Handlers
{
    public class UserDeletedEventHandler : IEventHandler<UserDeletedEvent>
    {
        private readonly UserRepository _userRepository;
        public UserDeletedEventHandler(IServiceProvider serviceProvider, UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task HandleAsync(UserDeletedEvent xEvent)
        {
            await _userRepository.DeleteUser(xEvent);
        }
    }
}