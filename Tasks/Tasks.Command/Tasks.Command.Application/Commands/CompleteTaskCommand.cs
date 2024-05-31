using Core.Messages;

namespace Tasks.Command.Application.Commands
{
    public class CompleteTaskCommand : BaseCommand
    {
        public Guid TaskId { get => Id; set => Id = value; }
        public Guid UserId { get; set; }
        public Guid CircleId { get; set; }
    }
}