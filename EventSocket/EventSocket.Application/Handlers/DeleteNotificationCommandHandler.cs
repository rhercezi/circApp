using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using EventSocket.Application.Commands;
using EventSocket.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace EventSocket.Application.Handlers
{
    public class DeleteNotificationCommandHandler : IMessageHandler<DeleteNotificationCommand>
    {
        private readonly NotificationRepository _notificationRepository;
        private readonly ILogger<DeleteNotificationCommandHandler> _logger;

        public DeleteNotificationCommandHandler(NotificationRepository notificationRepository,
                                                ILogger<DeleteNotificationCommandHandler> logger)
        {
            _notificationRepository = notificationRepository;
            _logger = logger;
        }

        public async Task<BaseResponse> HandleAsync(DeleteNotificationCommand message)
        {
            try
            {
                var result = await _notificationRepository.DeleteNotificationAsync(message.NotificationId);
                if (result.DeletedCount == 0)
                {
                    return new BaseResponse { ResponseCode = 404 };
                }
                return new BaseResponse { ResponseCode = 200 };
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return new BaseResponse { ResponseCode = 500 };
            }
        }

        public Task<BaseResponse> HandleAsync(BaseMessage message)
        {
            return HandleAsync((DeleteNotificationCommand)message);
        }
    }
}