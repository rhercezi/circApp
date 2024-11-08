using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using Core.Events;
using Core.MessageHandling;
using Core.Messages;
using EventSocket.Application.Commands;
using EventSocket.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EventSocket.Application.Services
{
    public class SocketConnectionManager
    {
        private readonly ILogger<SocketConnectionManager> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConnectedUsersService _connectedUsersService;
        public ConcurrentDictionary<string, WebSocket> _sockets = new();

        public SocketConnectionManager(ILogger<SocketConnectionManager> logger,
                                       IServiceScopeFactory scopeFactory,
                                       IConnectedUsersService connectedUsersService)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _connectedUsersService = connectedUsersService;
        }

        public async Task AddSocketAsync(Guid id, WebSocket socket)
        {
            _sockets.TryAdd(id.ToString(), socket);
            _logger.LogDebug("WebSocket connection added: {SocketId}", id);

            var buffer = new byte[1024 * 4];
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            SendCommand(id, CommandType.SendNotifications);
            _connectedUsersService.AddConnectedUser(id);

            while (!result.CloseStatus.HasValue)
            {
                try
                {
                    result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    if (Guid.TryParse(message, out var notificationId))
                    {
                        SendCommand(notificationId, CommandType.DeleteNotifications);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError("An exception occurred receiving ws message : {Message}\n{StackTrace}", e.Message, e.StackTrace);
                }
            }

            _sockets.TryRemove(id.ToString(), out _);
            _connectedUsersService.RemoveConnectedUser(id);
            await socket.CloseAsync(result.CloseStatus.Value,
                                    result.CloseStatusDescription,
                                    CancellationToken.None);
            _logger.LogDebug("WebSocket connection closed: {SocketId}", id);
        }

        private async void SendCommand(Guid id, CommandType commandType)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var consumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();
                var eventDispatcher = scope.ServiceProvider.GetRequiredService<IMessageDispatcher>();

                var command = commandType switch
                {
                    CommandType.SendNotifications => (BaseCommand)new SendNotificationsCommand { Id = id },
                    CommandType.DeleteNotifications => (BaseCommand)new DeleteNotificationCommand { NotificationId = id },
                    _ => throw new ArgumentOutOfRangeException($"Command type not found {commandType.ToString()}")
                };
                await eventDispatcher.DispatchAsync(command);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
            }
        }

        public async Task SendMessageAsync(List<Guid> ids, NotificationModel message)
        {
            if (message == null)
            {
                _logger.LogWarning("Class: {class}, Method: {method}, Message is null", GetType().FullName, nameof(SendMessageAsync));
                return;
            }
            var messageString = JsonConvert.SerializeObject(message);
            var buffer = Encoding.UTF8.GetBytes(messageString);
            var tasks = _sockets.Where(s => ids.Contains(Guid.Parse(s.Key)));
            foreach (var task in tasks)
            {
                if (task.Value.State == WebSocketState.Open)
                {
                    await task.Value.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length),
                                               WebSocketMessageType.Text,
                                               true,
                                               CancellationToken.None);
                    _logger.LogDebug("Message sent to WebSocket: {SocketId}", task.Key);
                }
                else
                {
                    _logger.LogDebug("WebSocket is not open: {SocketId}", task.Key);
                }
            }
        }

        public async Task SendMessageAsync(Guid id, NotificationModel message)
        {
            if (message == null)
            {
                _logger.LogWarning("Class: {class}, Method: {method}, Message is null", this.GetType().FullName, nameof(SendMessageAsync));
                return;
            }
            var messageString = JsonConvert.SerializeObject(message);
            var buffer = Encoding.UTF8.GetBytes(messageString);
            var tasks = _sockets.Where(s => s.Key == id.ToString());

            if (tasks.Any())
            {
                await tasks.First().Value.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length),
                                              WebSocketMessageType.Text,
                                              true,
                                              CancellationToken.None);
            }
        }

        private enum CommandType
        {
            SendNotifications = 0,
            DeleteNotifications = 1

        }
    }
}