using User.Command.Api.Commands;
using User.Command.Domin.Stores;

namespace User.Command.Application.Validation
{
    public class CreateUserCommandValidator : UserCommandValidatorBase<CreateUserCommand>
    {
        private List<string> _messages = new();

        public CreateUserCommandValidator(EventStore eventStore) : base(eventStore)
        {   
        }

        public override async Task ValidateCommand(CreateUserCommand command)
        {
            _messages.Add(ValidateNameString(command.UserName, "User name"));
            _messages.Add(ValidateNameString(command.FirstName, "First name"));
            _messages.Add(ValidateNameString(command.FamilyName, "Family name"));
            _messages.Add(ValidatePassword(command.Password));
            _messages.Add(await ValidateUserExistsByUsername(command.UserName, command.Id));
            _messages.Add(ValidateEmail(command.Email));
            _messages.Add(await ValidateUserExistsById(command.Id));

            ThrowIfErrorExists(_messages);
        }
    }
}