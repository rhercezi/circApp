using Core.Messages;

namespace EventSocket.Application.Commands
{
    public class MarkReminderAsSeenCommand : BaseCommand
    {
        public Guid ReminderId { get; set; }
    }
}