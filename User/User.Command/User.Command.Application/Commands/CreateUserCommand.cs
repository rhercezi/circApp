using Core.Messages;

namespace User.Command.Application.Commands
{
    public class CreateUserCommand : BaseCommand
    {
        public DateTime Created { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string FamilyName { get; set; }
        public string Email { get; set; }
        public CreateUserCommand(string userName,
                                    string password,
                                    string firstName,
                                    string familyName,
                                    string email)
        {
            UserName = userName;
            Password = password;
            FirstName = firstName;
            FamilyName	= familyName;
            Email = email;
            Id = Guid.NewGuid();
            Created = DateTime.Now;
        }
    }
}