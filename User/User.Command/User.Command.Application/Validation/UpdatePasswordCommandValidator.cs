using Core.Messages;
using User.Command.Application.Commands;
using User.Command.Domin.Stores;
using User.Common.PasswordService;

namespace User.Command.Application.Validation
{
    public class UpdatePasswordCommandValidator : UserCommandValidatorBase<UpdatePasswordCommand>
    {
        private List<string> _messages = new();
        private readonly List<BaseEvent> _events;
        private PasswordHashService _passwordHashService;

        public UpdatePasswordCommandValidator(EventStore eventStore, PasswordHashService passwordHashService, List<BaseEvent> events) : base(eventStore)
        {
            _events = events;
            _passwordHashService = passwordHashService;
        }

        public async override Task ValidateCommand(UpdatePasswordCommand command)
        {
            _messages.Add(ValidatePassword(command.Password));
            _messages.Add(ValidateOldPassword(command, _events, _passwordHashService));

            ThrowIfErrorExists(_messages);
        }
    }
}