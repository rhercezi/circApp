using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using User.Command.Application.Commands;
using User.Command.Domin.Stores;
using User.Common.Events;

namespace User.Command.Application.Validation
{
    public class UpdateUserCommandValidator : UserCommandValidatorBase<UserEditedEvent>
    {
        private List<string> _messages = new();

        public UpdateUserCommandValidator(EventStore eventStore) : base(eventStore)
        {
        }

        public override async Task ValidateCommand(UserEditedEvent xEvent)
        {
            _messages.Add(ValidateNameString(xEvent.UserName, "User name"));
            _messages.Add(ValidateNameString(xEvent.FirstName, "First name"));
            _messages.Add(ValidateNameString(xEvent.FamilyName, "Family name"));
            _messages.Add(ValidateEmail(xEvent.Email));
            _messages.Add(await ValidateUserExistsByUsername(xEvent.UserName, xEvent.Id));

            ThrowIfErrorExists(_messages);
        }
    }
}