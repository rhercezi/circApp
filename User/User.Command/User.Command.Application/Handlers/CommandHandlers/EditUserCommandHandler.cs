using Core.Events.PublicEvents;
using Core.MessageHandling;
using Core.Messages;
using User.Command.Api.Commands;
using User.Command.Application.Validation;
using User.Command.Domain.Aggregates;
using User.Command.Domain.Events;
using User.Command.Domain.Exceptions;
using User.Command.Domin.Stores;
using User.Common.Events;

namespace User.Command.Application.Handlers.CommandHandlers
{
    public class EditUserCommandHandler : ICommandHandler<EditUserCommand>
    {
        private readonly EventStore _eventStore;
        private readonly EventProducer _eventProducer;

        public EditUserCommandHandler(EventStore eventStore, EventProducer eventProducer)
        {
            _eventStore = eventStore;
            _eventProducer = eventProducer;
        }

        public async Task HandleAsync(EditUserCommand command)
        {
            var events = await _eventStore.GetEventsAsync(command.Id);
            if (events.Count == 0) throw new UserDomainException("No users found with the given ID.");
            UserAggregate userAggregate = new(_eventStore);
            userAggregate.ReplayEvents(events);
            var version = events.Max(e => e.Version) + 1;
            EditUserCommandValidator validator = new(_eventStore);

            if (string.IsNullOrEmpty(command.UserName) || string.IsNullOrEmpty(command.Email))
            {
                UpdateMissingValues(ref command, events);
            }

            await validator.ValidateCommand(command);

            userAggregate.InvokAction<UserEditedEvent>(
                new UserEditedEvent
                (
                    command.Id,
                    version,
                    command.UserName,
                    command.FirstName,
                    command.FamilyName,
                    command.Email,
                    false,
                    command.Updated
                )
            );

            _ = _eventProducer.ProduceAsync(
                new UserUpdatedPublicEvent
                (
                    command.Id,
                    command.UserName,
                    command.FirstName,
                    command.FamilyName,
                    command.Email
                ),
                "UserPublic"
            );
        }

        

        public async Task HandleAsync(BaseCommand command)
        {
            await HandleAsync((EditUserCommand)command);
        }

        private void UpdateMissingValues(ref EditUserCommand command, List<BaseEvent> events)
        {
            UserCreatedEvent userEvent = null;
            events.ForEach(e => {

                if(e is UserCreatedEvent event1)
                {
                    userEvent = event1;
                }
                else if (e is UserEditedEvent event2)
                {
                    if(!string.IsNullOrEmpty(event2.Email))
                    {
                        userEvent.Email = event2.Email;
                    }
                    if(!string.IsNullOrEmpty(event2.UserName))
                    {
                        userEvent.UserName = event2.UserName;
                    }
                }

            });

            if (string.IsNullOrEmpty(command.UserName)) command.UserName = userEvent.UserName;
            if (string.IsNullOrEmpty(command.Email)) command.Email = userEvent.Email;
        }
    }
}