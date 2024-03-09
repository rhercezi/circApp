using Circles.Domain.Repositories;
using Core.Events.PublicEvents;
using Core.MessageHandling;
using Core.Messages;

namespace Circles.Command.Application.Handlers
{
    public class UserDeletedEventHandler : IEventHandler<UserDeletedPublicEvent>
    {
        private readonly UserRepository _userRepository;
        private readonly UserCircleRepository _userCircleRepository;

        public UserDeletedEventHandler(UserRepository userRepository, UserCircleRepository userCircleRepository)
        {
            _userRepository = userRepository;
            _userCircleRepository = userCircleRepository;
        }

        public async Task HandleAsync(UserDeletedPublicEvent xEvent)
        {
            await _userRepository.DeleteUser(xEvent.Id);
            await _userCircleRepository.DeleteByUser(xEvent.Id);
        }

        public async Task HandleAsync(BaseEvent xEvent)
        {
            await HandleAsync((UserDeletedPublicEvent)xEvent);
        }
    }
}