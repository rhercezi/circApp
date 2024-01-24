using Core.MessageHandling;
using Microsoft.Extensions.DependencyInjection;
using User.Command.Api.Commands;
using User.Command.Application.Validation;
using User.Command.Domain.Aggregates;
using User.Command.Domin.Stores;
using User.Common.Events;

namespace User.Command.Application.Handlers.CommandHandlers
{
    public class EditUserCommandHandler : ICommandHandler<EditUserCommand>
    {
        private readonly EventStore _eventStore;

        public EditUserCommandHandler(IServiceProvider serviceProvider)
        {
            _eventStore = serviceProvider.GetRequiredService<EventStore>();
        }

        public async Task HandleAsync(EditUserCommand command)
        {
            var events = await _eventStore.GetEventsAsync(command.Id);
            UserAggregate userAggregate = new(_eventStore);
            userAggregate.ReplayEvents(events);
            var version = events.Max(e => e.Version) + 1;
            EditUserCommandValidator validator = new(_eventStore);

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
        }
    }
}