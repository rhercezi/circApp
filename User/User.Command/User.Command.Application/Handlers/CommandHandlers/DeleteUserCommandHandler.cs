using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.MessageHandling;
using Core.Repositories;
using User.Command.Api.Commands;
using User.Command.Domain.Aggregates;
using User.Command.Domin.Stores;
using User.Common.Events;

namespace User.Command.Application.Handlers.CommandHandlers
{
    public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
    {
        private readonly EventStore _eventStore;

        public DeleteUserCommandHandler(IServiceProvider serviceProvider)
        {
            _eventStore = serviceProvider.GetService(typeof(IEventStore)) as EventStore;
        }

        public Task HandleAsync(DeleteUserCommand command)
        {
            var events = _eventStore.GetEventsAsync(command.Id).Result;
            UserAggregate userAggregate = new(_eventStore);
            userAggregate.ReplayEvents(events);
            var version = events.Max(e => e.Version) + 1;

            userAggregate.InvokAction<UserDeletedEvent>(
                new UserDeletedEvent
                (
                    command.Id,
                    version
                )
            );
            return Task.FromResult(0);
        }
    }
}