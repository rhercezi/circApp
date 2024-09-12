using Core.Messages;

namespace Circles.Command.Application.Commands
{
    public class ConfirmJoinCommand : BaseCommand
    {
        public Guid UserId { get; set; }
        public Guid CircleId
        {
            get => Id;
            set => Id = value;
        }
        public bool IsAccepted { get; set; }
    }
}