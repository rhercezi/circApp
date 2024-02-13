using Core.Messages;

namespace User.Command.Application.Commands
{
    public class ResetPasswordCommand : BaseCommand
    {
        public string UserName { get; set; }

        public ResetPasswordCommand(string userName)
        {
            UserName = userName;
        }
    }
}