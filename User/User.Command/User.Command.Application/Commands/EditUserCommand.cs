using Core.Messages;

namespace User.Command.Api.Commands
{
    public class EditUserCommand : BaseCommand
    {
        public DateTime Updated { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? FamilyName { get; set; }
        public string? Email { get; set; }

        public EditUserCommand(Guid id, string userName, string firstName, string familyName, string email)
        {
            Updated = DateTime.Now;
            UserName = userName;
            FirstName = firstName;
            FamilyName = familyName;
            Email = email;
            Id = id;
        }
    }
}