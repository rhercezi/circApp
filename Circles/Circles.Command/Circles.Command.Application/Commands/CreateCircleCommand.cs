using Core.Messages;

namespace Circles.Command.Application.Commands
{
    public class CreateCircleCommand : BaseCommand
    {
        public CreateCircleCommand()
        {
            Id = Guid.NewGuid();
        }

        public Guid CreatorId { get; set; }
        public required string Name { get; set; }
        public required string Color { get; set; }
        public required List<Guid> Users { get; set; }
        public Guid CircleId { get => Id; }
    }
}