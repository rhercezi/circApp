using Core.DAOs;
using Core.Messages;

namespace EventSocket.Application.Commands
{
    public class SendReminderCommand : BaseCommand
    {
        public Guid UserId { get; set; }
        public ReminderModel Reminder { get; set; }

        public SendReminderCommand(Guid userId, ReminderModel reminder)
        {
            UserId = userId;
            Reminder = reminder;
        }
    }
}