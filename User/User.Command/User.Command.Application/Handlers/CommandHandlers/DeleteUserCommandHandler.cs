using Core.MessageHandling;
using Microsoft.Extensions.DependencyInjection;
using User.Command.Api.Commands;
using User.Command.Application.Validation;
using User.Command.Domain.Aggregates;
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

        public DeleteUserCommandHandler(IServiceProvider serviceProvider)
        {
            _eventStore = serviceProvider.GetRequiredService<EventStore>();
            _passwordHashService = serviceProvider.GetRequiredService<PasswordHashService>();
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
            
        }
    }
}