using Core.Messages;

namespace User.Command.Application.Commands
{
    public class UpdatePasswordCommand : BaseCommand
    {
        public DateTime Updated { get; set; }
        public string Password { get; set; }
        public string OldPassword { get; set; }

        public UpdatePasswordCommand(Guid id, string password, string oldPassword)
        {
            Updated = DateTime.Now;
            Id = id;
            Password = password;
            OldPassword = oldPassword;
        }
    }
}