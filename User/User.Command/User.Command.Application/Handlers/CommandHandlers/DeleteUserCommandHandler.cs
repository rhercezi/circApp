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
using User.Common.PasswordService;

namespace User.Command.Application.Handlers.CommandHandlers
{
    public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
    {
        private readonly EventStore _eventStore;
        private PasswordHashService _passwordHashService;
        private readonly EventProducer _eventProducer;

        public DeleteUserCommandHandler(EventStore eventStore, PasswordHashService passwordHashService, EventProducer eventProducer)
        {
            _eventStore = eventStore;
            _passwordHashService = passwordHashService;
            _eventProducer = eventProducer;
        }

        public async Task HandleAsync(DeleteUserCommand command)
        {
            var events = await _eventStore.GetEventsAsync(command.Id);
            if (events.Count == 0) throw new UserDomainException("No users found with the given ID.");
            UserAggregate userAggregate = new(_eventStore);
            userAggregate.ReplayEvents(events);
            var version = events.Max(e => e.Version) + 1;

            DeleteUserCommandValidator validator = new(_eventStore, events, _passwordHashService);

            await validator.ValidateCommand(command);
            
            userAggregate.InvokAction<UserDeletedEvent>(
                new UserDeletedEvent
                (
                    command.Id,
                    version
                )
            );

            _ = _eventProducer.ProduceAsync(
                new UserDeletedPublicEvent
                (
                    command.Id
                ),
                "UserPublic"
            );
            
        }

        public async Task HandleAsync(BaseCommand command)
        {
            await HandleAsync((DeleteUserCommand)command);
        }
    }
}