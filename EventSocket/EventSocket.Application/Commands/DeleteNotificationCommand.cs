using Core.Messages;

namespace EventSocket.Application.Commands
{
    public class DeleteNotificationCommand : BaseCommand
    {
        public Guid NotificationId { get; set; }
    }
}