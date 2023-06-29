using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Messages;

namespace User.Command.Api.Commands
{
    public class EditUserCommand : BaseCommand
    {
        public DateTime Updated { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string FamilyName { get; set; }
        public string Email { get; set; }

        public EditUserCommand(Guid id, string userName, string password, string firstName, string familyName, string email)
        {
            Updated = DateTime.Now;
            UserName = userName;
            Password = password;
            FirstName = firstName;
            FamilyName = familyName;
            Email = email;
            Id = id;
        }
    }
}