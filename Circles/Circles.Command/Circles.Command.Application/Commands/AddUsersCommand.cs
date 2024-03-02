using Core.Messages;

namespace Circles.Command.Application.Commands
{
    public class AddUsersCommand : BaseCommand
    {
        public required List<Guid> Users { get; set; }
        public required Guid CircleId
        {
            get => Id;
            set => Id = value;
        }
        public required Guid InviterId { get; set; }
    }
}