using User.Command.Api.Commands;
using User.Command.Domin.Stores;

namespace User.Command.Application.Validation
{
    public class EditUserCommandValidator : UserCommandValidatorBase<EditUserCommand>
    {
        private List<string> _messages = new();

        public EditUserCommandValidator(EventStore eventStore) : base(eventStore)
        {
        }

        public override async Task ValidateCommand(EditUserCommand command)
        {
            if(!string.IsNullOrEmpty(command.UserName))
            {
                _messages.Add(ValidateNameString(command.UserName, "User name"));
                _messages.Add(await ValidateUserExistsByUsername(command.UserName, command.Id));
            }
            if(!string.IsNullOrEmpty(command.FirstName))
            {
                _messages.Add(ValidateNameString(command.FirstName, "First name"));
            }
            if(!string.IsNullOrEmpty(command.FamilyName))
            {
                _messages.Add(ValidateNameString(command.FamilyName, "Familly name"));
            }
            if(!string.IsNullOrEmpty(command.Email))
            {
                _messages.Add(ValidateEmail(command.Email));
            }

            ThrowIfErrorExists(_messages);
        }
    }
}