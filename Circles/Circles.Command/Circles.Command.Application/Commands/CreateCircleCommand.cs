using Core.Messages;

namespace Circles.Command.Application.Commands
{
    public class CreateCircleCommand : BaseCommand
    {
        public Guid CreatorId { get; set; }
        public required string Name { get; set; }
        public required string Color { get; set; }
        public List<Guid>? Users { get; set; }
        public Guid CircleId { get => Id; }
    }
}