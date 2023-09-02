using Core.MessageHandling;
using Core.Repositories;
using User.Command.Api.Commands;
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
            _eventStore = serviceProvider.GetService(typeof(IEventStore)) as EventStore;
        }

        public Task HandleAsync(EditUserCommand command)
        {
            var events = _eventStore.GetEventsAsync(command.Id).Result;
            UserAggregate userAggregate = new(_eventStore);
            userAggregate.ReplayEvents(events);
            var version = events.Max(e => e.Version) + 1;

            userAggregate.InvokAction<UserEditedEvent>(
                new UserEditedEvent
                (
                    command.Id,
                    version,
                    command.UserName,
                    command.Password,
                    command.FirstName,
                    command.FamilyName,
                    command.Email,
                    false,
                    command.Updated
                )
            );
            return Task.FromResult(0);
        }
    }
}