using Core.Messages;

namespace User.Command.Application.Commands
{
    public class VerifyEmailCommand : BaseCommand
    {
        public string IdLink { get; set; }

        public VerifyEmailCommand(string idLink)
        {
            this.IdLink = idLink;
        }
    }
}