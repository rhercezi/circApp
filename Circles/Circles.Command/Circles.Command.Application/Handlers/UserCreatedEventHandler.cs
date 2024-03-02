using Circles.Domain.Entities;
using Circles.Domain.Repositories;
using Core.Events.PublicEvents;
using Core.MessageHandling;
using Core.Messages;

namespace Circles.Command.Application.Handlers
{
    public class UserCreatedEventHandler : IEventHandler<UserCreatedPublicEvent>
    {
        private readonly UserRepository _userRepository;

        public UserCreatedEventHandler(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task HandleAsync(UserCreatedPublicEvent xEvent)
        {
            await _userRepository.SaveAsync(
                new AppUserModel
                {
                    UserId = xEvent.Id,
                    UserName = xEvent.UserName,
                    FirstName = xEvent.FirstName,
                    FamilyName = xEvent.FamilyName,
                    Email = xEvent.Email
                }
            );

        }

        public async Task HandleAsync(BaseEvent xEvent)
        {
            await HandleAsync((UserCreatedPublicEvent)xEvent);
        }
    }
}