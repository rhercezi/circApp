using Core.Messages;

namespace Circles.Command.Application.Commands
{
    public class RemoveUsersCommand : BaseCommand
    {
        public required List<Guid> Users { get; set; }
        public Guid CircleId
        {
            get => Id;
            set => Id = value;
        }
    }
}