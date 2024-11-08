using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using EventSocket.Application.Commands;
using EventSocket.Application.Services;
using EventSocket.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace EventSocket.Application.Handlers
{
    public class SendNotificationsCommandHandler : IMessageHandler<SendNotificationsCommand>
    {
        private readonly NotificationRepository _notificationRepository;
        private readonly SocketConnectionManager _socketConnectionManager;
        private readonly ILogger<SendNotificationsCommandHandler> _logger;

        public SendNotificationsCommandHandler(NotificationRepository notificationRepository,
                                               SocketConnectionManager socketConnectionManager,
                                               ILogger<SendNotificationsCommandHandler> logger)
        {
            _notificationRepository = notificationRepository;
            _socketConnectionManager = socketConnectionManager;
            _logger = logger;
        }

        public async Task<BaseResponse> HandleAsync(SendNotificationsCommand message)
        {
            try
            {
                var notifications = await _notificationRepository.GetNotificationsByUserIdAsync(message.Id);
                foreach (var notification in notifications)
                {
                    await _socketConnectionManager.SendMessageAsync(message.Id, notification);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return new BaseResponse { ResponseCode = 500 };
            }
            return new BaseResponse { ResponseCode = 200 };
        }

        public Task<BaseResponse> HandleAsync(BaseMessage message)
        {
            return HandleAsync((SendNotificationsCommand)message);
        }
    }
}