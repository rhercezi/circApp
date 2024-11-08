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
    public class JoinRequestEventHandler : IMessageHandler<JoinCircleRequestPublicEvent>
    {
        private readonly InternalHttpClient<CircleDto> _internalHttp;
        private readonly IOptions<CirclesServiceConfig> _config;
        private readonly ILogger<JoinRequestEventHandler> _logger;
        private readonly SocketConnectionManager _socketConnectionManager;
        private readonly NotificationRepository _notificationRepository;

        public JoinRequestEventHandler(InternalHttpClient<CircleDto> internalHttp,
                                       IOptions<CirclesServiceConfig> config,
                                       ILogger<JoinRequestEventHandler> logger,
                                       SocketConnectionManager socketConnectionManager,
                                       NotificationRepository notificationRepository)
        {
            _internalHttp = internalHttp;
            _config = config;
            _logger = logger;
            _socketConnectionManager = socketConnectionManager;
            _notificationRepository = notificationRepository;
        }

        public async Task<BaseResponse> HandleAsync(JoinCircleRequestPublicEvent message)
        {
            if (message != null)
            {
                var clientConfig = new HttpClientConfig
                {
                    BaseUrl = _config.Value.BaseUrl,
                    Path = _config.Value.Path + message.CircleId.ToString(),
                    Port = _config.Value.Port
                };

                CircleDto circleDto = new();
                try
                {
                    circleDto = await _internalHttp.GetResource(clientConfig);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error getting circle from CirclesService");
                    return new BaseResponse { ResponseCode = 500 };
                }

                if (circleDto != null)
                {
                    var inviter = circleDto.Users.Find(u => u.Id == message.InviterId);
                    if (inviter == null)
                    {
                        return new BaseResponse { ResponseCode = 404 };
                    }

                    var messageToSend = $"User {inviter.FirstName} {inviter.FamilyName} ({inviter.UserName}) wants you to join the circle {circleDto.Name}.";
                    var notification = new NotificationModel
                    {
                        Id = Guid.NewGuid(),
                        UserId = message.UserId,
                        IsRead = false,
                        Body = new NotificationBodyModel
                        {
                            TargetId = message.CircleId,
                            Message = messageToSend,
                            Type = NotificationType.JoinRequest
                        }
                    };
                    _logger.LogDebug("Created message {Message} for circle {user}", messageToSend, message.UserId);

                    await _notificationRepository.AddNotificationAsync(notification);
                    await _socketConnectionManager.SendMessageAsync(message.UserId, notification);
                    return new BaseResponse { ResponseCode = 200 };
                }
            }
            return new BaseResponse { ResponseCode = 500 };
        }

        public Task<BaseResponse> HandleAsync(BaseMessage message)
        {
            return HandleAsync((JoinCircleRequestPublicEvent)message);
        }
    }
}