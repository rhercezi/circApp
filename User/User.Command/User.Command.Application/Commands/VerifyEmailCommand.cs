using Core.Messages;

namespace User.Command.Application.Commands
{
    public class VerifyEmailCommand : BaseCommand
    {
        public string idLink { get; set; }

        public VerifyEmailCommand(string idLink)
        {
            this.idLink = idLink;
        }
    }
}