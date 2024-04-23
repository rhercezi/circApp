using Core.Messages;

namespace Tasks.Command.Application.Commands
{
    public class CompleteTaskCommand : BaseCommand
    {
        public Guid UserId { get; set; }
        public Guid CircleId { get; set; }
    }
}