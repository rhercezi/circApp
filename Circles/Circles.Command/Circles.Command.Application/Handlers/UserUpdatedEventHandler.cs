using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Circles.Domain.Entities;
using Circles.Domain.Repositories;
using Core.Events.PublicEvents;
using Core.MessageHandling;
using Core.Messages;

namespace Circles.Command.Application.Handlers
{
    public class UserUpdatedEventHandler : IEventHandler<UserUpdatedPublicEvent>
    {
        private readonly UserRepository _userRepository;
        public UserUpdatedEventHandler(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task HandleAsync(UserUpdatedPublicEvent xEvent)
        {
            await _userRepository.UpdateUser(
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
            await HandleAsync((UserUpdatedPublicEvent)xEvent);
        }
    }
}