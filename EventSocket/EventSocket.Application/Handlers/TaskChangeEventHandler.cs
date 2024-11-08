using Core.Configs;
using Core.DTOs;
using Core.Events.PublicEvents;
using Core.MessageHandling;
using Core.Messages;
using Core.Utilities;
using EventSocket.Application.Config;
using EventSocket.Application.Services;
using EventSocket.Domain.Entities;
using EventSocket.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventSocket.Application.Handlers
{
    public class TaskChangeEventHandler : IMessageHandler<TaskChangePublicEvent>
    {
        private readonly InternalHttpClient<CircleDto> _internalHttp;
        private readonly IOptions<CirclesServiceConfig> _config;
        private readonly NotificationRepository _notificationRepository;
        private readonly SocketConnectionManager _socketConnectionManager;
        private readonly ILogger<TaskChangeEventHandler> _logger;

        public TaskChangeEventHandler(InternalHttpClient<CircleDto> internalHttp,
                                      NotificationRepository notificationRepository,
                                      IOptions<CirclesServiceConfig> config,
                                      SocketConnectionManager socketConnectionManager,
                                      ILogger<TaskChangeEventHandler> logger)
        {
            _internalHttp = internalHttp;
            _notificationRepository = notificationRepository;
            _config = config;
            _socketConnectionManager = socketConnectionManager;
            _logger = logger;
        }

        public async Task<BaseResponse> HandleAsync(TaskChangePublicEvent message)
        {
            if (message != null)
            {
                if (message.CircleId != null)
                {
                    var clientConfig = new HttpClientConfig
                    {
                        BaseUrl = _config.Value.BaseUrl,
                        Path = _config.Value.Path + message.CircleId.ToString(),
                        Port = _config.Value.Port
                    };

                    var circleDto = await _internalHttp.GetResource(clientConfig);
                    if (circleDto != null)
                    {
                        message.UserIds ??= new List<Guid>();
                        message.UserIds.AddRange(circleDto.Users.Select(u => u.Id));
                    }

                    if (message.UserIds != null && message.UserIds.Count > 0)
                    {
                        return await SendNotifications(message);
                    }
                    _logger.LogError("No users found for task {TaskId}", message.TaskId);
                    return new BaseResponse { ResponseCode = 500 };
                }
                else if (message.UserIds != null && message.UserIds.Count > 0)
                {
                    return await SendNotifications(message);
                }
                _logger.LogError("No users found for task {TaskId}", message.TaskId);
            }
            _logger.LogError("TaskChangePublicEvent is null");
            return new BaseResponse { ResponseCode = 500 };
        }

        private async Task<BaseResponse> SendNotifications(TaskChangePublicEvent message)
        {
            if (message.UserIds == null)
            {
                _logger.LogError("UserIds is null for task {TaskId}", message.TaskId);
                return new BaseResponse { ResponseCode = 500 };
            }

            foreach (var user in message.UserIds)
            {
                var messageToSend = message.Action switch
                {
                    EventType.Create => $"Task {message.Title}, {message.Date.ToString("f")} has been created.",
                    EventType.Update => $"Task {message.Title}, {message.Date.ToString("f")} has been updated.",
                    EventType.Delete => $"Task {message.Title}, {message.Date.ToString("f")} has been deleted.",
                    _ => string.Empty
                };
                _logger.LogDebug("Created message {Message} for task {TaskId}", messageToSend, message.TaskId);

                var notification = new NotificationModel
                {
                    Id = Guid.NewGuid(),
                    UserId = user,
                    IsRead = false,
                    Body = new NotificationBodyModel
                    {
                        TargetId = message.TaskId,
                        Message = messageToSend,
                        Type = NotificationType.Task
                    }
                };
                _logger.LogDebug("Adding notification {Notification} for task {TaskId}", notification, message.TaskId);

                await _notificationRepository.AddNotificationAsync(notification);
                await _socketConnectionManager.SendMessageAsync(user, notification);
            }
            return new BaseResponse { ResponseCode = 200 };
        }

        public Task<BaseResponse> HandleAsync(BaseMessage message)
        {
            return HandleAsync((TaskChangePublicEvent)message);
        }
    }
}