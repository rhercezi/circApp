using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using EventSocket.Application.Commands;
using EventSocket.Application.Services;
using EventSocket.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EventSocket.Application.Handlers
{
    public class SendReminderCommandHandler : IMessageHandler<SendReminderCommand>
    {
        private readonly SocketConnectionManager _socketConnectionManager;

        public SendReminderCommandHandler(SocketConnectionManager socketConnectionManager)
        {
            _socketConnectionManager = socketConnectionManager;
        }

        public async Task<BaseResponse> HandleAsync(SendReminderCommand message)
        {
            if (message != null)
            {
                var reminder = new NotificationModel
                {
                    Id = Guid.NewGuid(),
                    UserId = message.Id,
                    IsRead = false,
                    Body = new NotificationBodyModel
                    {
                        TargetId = message.Reminder.AppointmentId,
                        Message = message.Reminder.Message?? "Reminder " + message.Reminder.Time,
                        Type = NotificationType.Reminder
                    }
                };
                await _socketConnectionManager.SendMessageAsync(message.Id, reminder);
                return new BaseResponse { ResponseCode = 200 };
            }
            return new BaseResponse { ResponseCode = 500 };
        }

        public Task<BaseResponse> HandleAsync(BaseMessage message)
        {
            return HandleAsync((SendReminderCommand)message);
        }
    }
}