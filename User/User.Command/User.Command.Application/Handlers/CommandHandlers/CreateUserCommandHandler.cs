using Core.MessageHandling;
using Microsoft.Extensions.DependencyInjection;
using User.Command.Api.Commands;
using User.Command.Application.Validation;
using User.Command.Domain.Aggregates;
using User.Command.Domin.Stores;
using User.Common.Events;
using User.Common.PasswordService;

namespace User.Command.Application.Handlers.CommandHandlers
{
    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
    {
        private readonly EventStore _eventStore;
        private PasswordHashService _passwordHashService;
        public CreateUserCommandHandler(IServiceProvider serviceProvider)
        {
            _eventStore = serviceProvider.GetRequiredService<EventStore>();
            _passwordHashService = serviceProvider.GetRequiredService<PasswordHashService>();
        }

        public async Task HandleAsync(CreateUserCommand command)
        {

            CreateUserCommandValidator validator = new(_eventStore);

            await validator.ValidateCommand(command);

            var hash = _passwordHashService.HashPassword(command.Password, command.Id);
            UserAggregate userAggregate = new(_eventStore);
            userAggregate.InvokAction<UserCreatedEvent>(
                new UserCreatedEvent
                (
                    command.Id,
                    0,
                    command.UserName,
                    hash,
                    command.FirstName,
                    command.FamilyName,
                    command.Email,
                    false,
                    command.Created
                )
            );
        }
    }
}