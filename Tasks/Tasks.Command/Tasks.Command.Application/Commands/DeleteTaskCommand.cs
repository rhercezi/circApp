using Core.Messages;

namespace Tasks.Command.Application.Commands
{
    public class DeleteTaskCommand : BaseCommand
    {
        public Guid OwnerId { get; set; }
    }
}