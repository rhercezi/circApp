using Core.Messages;
using User.Command.Api.Commands;
using User.Command.Domin.Stores;
using User.Common.PasswordService;

namespace User.Command.Application.Validation
{
    public class DeleteUserCommandValidator : UserCommandValidatorBase<DeleteUserCommand>
    {
        private List<string> _messages = new();
        private readonly List<BaseEvent> _events;
        private PasswordHashService _passwordHashService;

        public DeleteUserCommandValidator(EventStore eventStore, List<BaseEvent> events, PasswordHashService passwordHashService) : base(eventStore)
        {
            _events = events;
            _passwordHashService = passwordHashService;
        }

        public async override Task ValidateCommand(DeleteUserCommand command)
        {
            _messages.Add(ValidateOldPassword(command, _events, _passwordHashService));

            ThrowIfErrorExists(_messages);
        }
    }
}