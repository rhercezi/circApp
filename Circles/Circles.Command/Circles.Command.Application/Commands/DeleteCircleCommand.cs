using Core.Messages;

namespace Circles.Command.Application.Commands
{
    public class DeleteCircleCommand : BaseCommand
    {
        public Guid CircleId
        {
            get => Id; 
            set => Id = value; 
        }
    }
}