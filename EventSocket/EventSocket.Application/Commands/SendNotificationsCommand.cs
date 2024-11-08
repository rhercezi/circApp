using Core.Messages;

namespace EventSocket.Application.Commands
{
    public class SendNotificationsCommand : BaseCommand
    {
        public Guid UserId { get; set; }
    }
}