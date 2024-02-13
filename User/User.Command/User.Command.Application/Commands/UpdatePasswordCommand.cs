using Core.Messages;

namespace User.Command.Application.Commands
{
    public class UpdatePasswordCommand : BaseCommand
    {
        public DateTime Updated { get; set; }
        public string? Password { get; set; }
        public string? OldPassword { get; set; }
        public string? IdLink { get; set; }
    }
}