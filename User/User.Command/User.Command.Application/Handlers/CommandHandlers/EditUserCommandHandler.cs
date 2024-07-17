using Core.DTOs;
using Core.Events.PublicEvents;
using Core.MessageHandling;
using Core.Messages;
using User.Command.Application.Commands;
using User.Command.Domain.Aggregates;
using User.Command.Domain.Events;
using User.Command.Domin.Stores;
using User.Common.Events;

namespace User.Command.Application.Handlers.CommandHandlers
{
    public class EditUserCommandHandler : IMessageHandler<EditUserCommand>
    {
        private readonly EventStore _eventStore;
        private readonly EventProducer _eventProducer;

        public EditUserCommandHandler(EventStore eventStore, EventProducer eventProducer)
        {
            _eventStore = eventStore;
            _eventProducer = eventProducer;
        }

        public async Task<BaseResponse> HandleAsync(EditUserCommand command)
        {
            var events = await _eventStore.GetEventsAsync(command.Id);
            if (events.Count == 0) return new BaseResponse{ResponseCode=404, Message="No users found with the given ID."};
            UserAggregate userAggregate = new(_eventStore);
            userAggregate.ReplayEvents(events);
            var version = events.Max(e => e.Version) + 1;

            var userEvent = GetCurrentUser(ref command, events);

            if (userEvent == null) return new BaseResponse{ResponseCode=404, Message="No users found with the given ID."};

            var userToUpdate = new UserEditedEvent
                (
                    userEvent.Id,
                    version,
                    userEvent.UserName,
                    userEvent.FirstName,
                    userEvent.FamilyName,
                    userEvent.Email,
                    false,
                    command.Updated
                );

            command.UpdateJson.ApplyTo(userToUpdate);

            userAggregate.InvokAction<UserEditedEvent>(
                userToUpdate
            );

            _ = _eventProducer.ProduceAsync(
                new UserUpdatedPublicEvent
                (
                    userToUpdate.Id,
                    userToUpdate.UserName,
                    userToUpdate.FirstName,
                    userToUpdate.FamilyName,
                    userToUpdate.Email
                ),
                "user_public"
            );
            return new BaseResponse
            {
                ResponseCode = 200,
                Data = new UserDto
                {
                    Id = userToUpdate.Id,
                    UserName = userToUpdate.UserName,
                    FirstName = userToUpdate.FirstName,
                    FamilyName = userToUpdate.FamilyName,
                    Email = userToUpdate.Email,
                    Updated = userToUpdate.Updated
                }
            };
        }

        

        public async Task<BaseResponse> HandleAsync(BaseMessage command)
        {
            return await HandleAsync((EditUserCommand)command);
        }

        private static UserCreatedEvent? GetCurrentUser(ref EditUserCommand command, List<BaseEvent> events)
        {
            UserCreatedEvent? userEvent = null;
            events.ForEach(e => {

                if(e is UserCreatedEvent event1)
                {
                    userEvent = event1;
                }
                else if (e is UserEditedEvent event2)
                {
                    if(userEvent != null && !string.IsNullOrEmpty(event2.Email))
                    {
                        userEvent.Email = event2.Email;
                    }
                    if(userEvent != null && !string.IsNullOrEmpty(event2.UserName))
                    {
                        userEvent.UserName = event2.UserName;
                    }
                }

            });

            return userEvent;
        }
    }
}