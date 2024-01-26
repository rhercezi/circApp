using Core.MessageHandling;
using Microsoft.Extensions.DependencyInjection;
using User.Command.Application.Commands;
using User.Command.Application.Exceptions;
using User.Command.Domain.Aggregates;
using User.Command.Domin.Stores;
using User.Common.Events;
using User.Common.Utility;

namespace User.Command.Application.Handlers.CommandHandlers
{
    public class VerifyEmailCommandHandler : ICommandHandler<VerifyEmailCommand>
    {
        private readonly EventStore _eventStore;

        public VerifyEmailCommandHandler(IServiceProvider serviceProvider)
        {
            _eventStore = serviceProvider.GetRequiredService<EventStore>();
        }

        public async Task HandleAsync(VerifyEmailCommand command)
        {
            IdLinkConverter converter = new();

            command.Id = converter.IdLinkToGuid(command.idLink);
            var events = await _eventStore.GetEventsAsync(command.Id);

            if (events.Count == 0)
            {
                throw new UserValidationException("Invalid email confirmation link");
            }

            UserAggregate userAggregate = new(_eventStore);
            userAggregate.ReplayEvents(events);
            var version = events.Max(e => e.Version) + 1;

            userAggregate.InvokAction<EmailVerifiedEvent>(
                new EmailVerifiedEvent(
                    command.Id,
                    version
                )
            );
        }
    }
}