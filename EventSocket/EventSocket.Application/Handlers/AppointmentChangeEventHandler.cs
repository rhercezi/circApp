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
    public class AppointmentChangeEventHandler : IMessageHandler<AppointmentChangePublicEvent>
    {
        private readonly InternalHttpClient<CircleDto> _internalHttp;
        private readonly IOptions<CirclesServiceConfig> _config;
        private readonly NotificationRepository _notificationRepository;
        private readonly ILogger<AppointmentChangeEventHandler> _logger;
        private readonly SocketConnectionManager _socketConnectionManager;

        public AppointmentChangeEventHandler(InternalHttpClient<CircleDto> internalHttp,
                                             IOptions<CirclesServiceConfig> config,
                                             NotificationRepository notificationRepository,
                                             ILogger<AppointmentChangeEventHandler> logger,
                                             SocketConnectionManager socketConnectionManager)
        {
            _internalHttp = internalHttp;
            _config = config;
            _notificationRepository = notificationRepository;
            _logger = logger;
            _socketConnectionManager = socketConnectionManager;
        }

        public async Task<BaseResponse> HandleAsync(AppointmentChangePublicEvent message)
        {
            if (message != null)
            {
                if (message.Circles != null)
                {
                    List<Guid> users = new();
                    foreach (var circle in message.Circles)
                    {
                        var clientConfig = new HttpClientConfig
                        {
                            BaseUrl = _config.Value.BaseUrl,
                            Path = _config.Value.Path + circle.ToString(),
                            Port = _config.Value.Port
                        };

                        var circleDto = await _internalHttp.GetResource(clientConfig);
                        if (circleDto != null)
                        {
                            users.AddRange(circleDto.Users.Select(u => u.Id));
                        }

                        if (users.Count > 0)
                        {
                            foreach (var user in users)
                            {
                                var messageToSend = message.Action switch
                                {
                                    EventType.Create => $"Appointment {message.Title}, {message.Date.ToString("f")} has been created.",
                                    EventType.Update => $"Appointment {message.Title}, {message.Date.ToString("f")} has been updated.",
                                    EventType.Delete => $"Appointment {message.Title}, {message.Date.ToString("f")} has been deleted.",
                                    _ => string.Empty
                                };
                                _logger.LogDebug("Created message {Message} for task {TaskId}", messageToSend, message.AppointmentId);

                                var notification = new NotificationModel
                                {
                                    Id = Guid.NewGuid(),
                                    UserId = user,
                                    IsRead = false,
                                    Body = new NotificationBodyModel
                                    {
                                        TargetId = message.AppointmentId,
                                        Message = messageToSend,
                                        Type = NotificationType.Appointment
                                    }
                                };
                                _logger.LogDebug("Adding notification {Notification} for task {TaskId}", notification, message.AppointmentId);

                                try
                                {
                                    await _notificationRepository.AddNotificationAsync(notification);
                                    await _socketConnectionManager.SendMessageAsync(user, notification);
                                }
                                catch (Exception e)
                                {
                                    _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                                    return new BaseResponse { ResponseCode = 500 };
                                }
                            }
                            return new BaseResponse { ResponseCode = 200 };
                        }
                    }
                }
            }

            return new BaseResponse { ResponseCode = 500 };
        }

        public Task<BaseResponse> HandleAsync(BaseMessage message)
        {
            return HandleAsync((AppointmentChangePublicEvent)message);
        }
    }
}