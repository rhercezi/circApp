using Core.Messages;
using EventSocket.Application.DTOs;

namespace EventSocket.Application.Commands
{
    public class SendReminderCommand : BaseCommand
    {
        public Guid UserId { get; set; }
        public ReminderDto Reminder { get; set; }

        public SendReminderCommand(Guid userId, ReminderDto reminder)
        {
            UserId = userId;
            Reminder = reminder;
        }
    }
}