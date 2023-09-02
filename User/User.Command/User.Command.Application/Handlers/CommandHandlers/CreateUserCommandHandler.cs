using Core.MessageHandling;
using Core.Repositories;
using User.Command.Api.Commands;
using User.Command.Domain.Aggregates;
using User.Command.Domin.Stores;
using User.Common.Events;

namespace User.Command.Application.Handlers.CommandHandlers
{
    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
    {
        private readonly EventStore _eventStore;
        public CreateUserCommandHandler(IServiceProvider serviceProvider)
        {
            _eventStore = serviceProvider.GetService(typeof(IEventStore)) as EventStore;
        }

        public Task HandleAsync(CreateUserCommand command)
        {
            UserAggregate userAggregate = new(_eventStore);
            userAggregate.InvokAction<UserCreatedEvent>(
                new UserCreatedEvent
                (
                    command.Id,
                    0,
                    command.UserName,
                    command.Password,
                    command.FirstName,
                    command.FamilyName,
                    command.Email,
                    false,
                    command.Created
                )
            );
            return Task.FromResult(0);
        }
    }
}