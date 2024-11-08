using Core.Configs;
using Core.MessageHandling;
using Core.Utilities;
using EventSocket.Application.Commands;
using EventSocket.Application.Config;
using EventSocket.Application.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventSocket.Application.Services
{
    public class ReminderHostedService : BackgroundService
    {
        private readonly InternalHttpClient<List<ReminderDto>> _internalHttp;
        private readonly IOptions<RemindersServiceConfig> _config;
        private readonly ILogger<ReminderHostedService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConnectedUsersService _connectedUsersService;

        public ReminderHostedService(InternalHttpClient<List<ReminderDto>> internalHttp,
                                     IOptions<RemindersServiceConfig> config,
                                     ILogger<ReminderHostedService> logger,
                                     IServiceScopeFactory scopeFactory,
                                     IConnectedUsersService connectedUsersService)
        {
            _internalHttp = internalHttp;
            _config = config;
            _logger = logger;
            _scopeFactory = scopeFactory;
            _connectedUsersService = connectedUsersService;
            _connectedUsersService.UserAdded += OnUserAdded;
        }

        private void OnUserAdded(object? sender, Guid id)
        {
            SendMessageToNewUser(id);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Reminder hosted service started.");
            stoppingToken.Register(() => _logger.LogInformation("Reminder hosted service stopping."));

            Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var commandDispatcher = scope.ServiceProvider.GetRequiredService<IMessageDispatcher>();

                        _ = SendReminders(commandDispatcher);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                    }
                    await Task.Delay(300000, stoppingToken);
                }
            }, stoppingToken);

            _logger.LogInformation("Reminder hosted service stopped.");

            return Task.CompletedTask;
        }

        private async void SendMessageToNewUser(Guid id)
        {
            using var scope = _scopeFactory.CreateScope();
            var commandDispatcher = scope.ServiceProvider.GetRequiredService<IMessageDispatcher>();

            _ = DoSend(id, commandDispatcher);
        }

        private async Task SendReminders(IMessageDispatcher commandDispatcher)
        {
            foreach (var user in _connectedUsersService.GetConnectedUsers())
            {
                _ = DoSend(user, commandDispatcher);
            }
        }

        private async Task DoSend(Guid user, IMessageDispatcher commandDispatcher)
        {
            HttpClientConfig clientConfig;
            try
            {
                clientConfig = new HttpClientConfig
                {
                    BaseUrl = _config.Value.BaseUrl,
                    Path = _config.Value.Path + user + "?dateFrom=" + DateTime.Now.AddMinutes(1).ToString("yyyy-MM-ddTHH:mm:ss") + "&dateTo=" + DateTime.Now.AddMinutes(6).ToString("yyyy-MM-ddTHH:mm:ss"),
                    Port = _config.Value.Port
                };
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return;
            }

            try
            {
                var reminders = await _internalHttp.GetResource(clientConfig);

                if (reminders != null)
                {
                    foreach (var reminder in reminders)
                    {
                        try
                        {
                            await commandDispatcher.DispatchAsync(new SendReminderCommand(user, reminder));
                        }
                        catch (Exception e)
                        {
                            _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                            continue;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return;
            }
        }
    }
}