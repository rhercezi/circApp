using Core.MessageHandling;
using Microsoft.Extensions.DependencyInjection;
using User.Command.Application.Commands;
using User.Command.Application.Validation;
using User.Command.Domain.Aggregates;
using User.Command.Domin.Stores;
using User.Common.Events;
using User.Common.PasswordService;

namespace User.Command.Application.Handlers.CommandHandlers
{
    public class UpdatePasswordCommandHandler : ICommandHandler<UpdatePasswordCommand>
    {
        private readonly EventStore _eventStore;
        private PasswordHashService _passwordHashService;
        
        public UpdatePasswordCommandHandler(IServiceProvider serviceProvider)
        {
            _eventStore = serviceProvider.GetRequiredService<EventStore>();
            _passwordHashService = serviceProvider.GetRequiredService<PasswordHashService>();
        }

        public async Task HandleAsync(UpdatePasswordCommand command)
        {
            var events = await _eventStore.GetEventsAsync(command.Id);
            UserAggregate userAggregate = new(_eventStore);
            userAggregate.ReplayEvents(events);
            var version = events.Max(e => e.Version) + 1;

            UpdatePasswordCommandValidator validator = new(_eventStore, _passwordHashService, events);

            await validator.ValidateCommand(command);

            userAggregate.InvokAction<PasswordUpdatedEvent>(
                new PasswordUpdatedEvent
                (
                    command.Id,
                    version,
                    _passwordHashService.HashPassword(command.Password, command.Id),
                    command.Updated
                )
            );

        }
    }
}