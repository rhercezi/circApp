using Core.Messages;

namespace Circles.Command.Application.Commands
{
    public class UpdateCircleCommand : BaseCommand
    {
        public required string Name { get; set; }
        public required string Color { get; set; }
        public Guid CircleId
        {
            get => Id;
            set => Id = value; 
        }
    }
}