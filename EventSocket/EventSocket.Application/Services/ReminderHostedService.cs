using Core.Configs;
using Core.DAOs;
using Core.DTOs;
using Core.MessageHandling;
using Core.Utilities;
using EventSocket.Application.Commands;
using EventSocket.Application.Config;
using EventSocket.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventSocket.Application.Services
{
    public class ReminderHostedService : BackgroundService
    {
        private readonly InternalHttpClient<AppUserDto> _internalHttp;
        private readonly IOptions<UserCirclesServiceConfig> _config;
        private readonly ILogger<ReminderHostedService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConnectedUsersService _connectedUsersService;
        private readonly ReminderRepository _reminderRepository;

        public ReminderHostedService(InternalHttpClient<AppUserDto> internalHttp,
                                     IOptions<UserCirclesServiceConfig> config,
                                     ILogger<ReminderHostedService> logger,
                                     IServiceScopeFactory scopeFactory,
                                     IConnectedUsersService connectedUsersService,
                                     ReminderRepository reminderRepository)
        {
            _internalHttp = internalHttp;
            _config = config;
            _logger = logger;
            _scopeFactory = scopeFactory;
            _connectedUsersService = connectedUsersService;
            _connectedUsersService.UserAdded += OnUserAdded;
            _reminderRepository = reminderRepository;
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
                        _ = SendReminders();
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

            _ = DoSend(id);
        }

        private async Task SendReminders()
        {
            foreach (var user in _connectedUsersService.GetConnectedUsers())
            {
                _ = DoSend(user);
            }
        }

        private async Task DoSend(Guid user)
        {
            HttpClientConfig clientConfig;
            try
            {
                clientConfig = new HttpClientConfig
                {
                    BaseUrl = _config.Value.BaseUrl,
                    Path = _config.Value.Path + user.ToString(),
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
                var appUserDto = await _internalHttp.GetResource(clientConfig);

                var reminders = await GetRemindersForAppUser(appUserDto);

                if (reminders != null)
                {
                    _ = SendRemindersAsync(user, reminders);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return;
            }
        }

        private async Task SendRemindersAsync(Guid user, List<ReminderModel> reminders)
        {
            var tasks = reminders.Select(async r =>
            {
                var command = new SendReminderCommand(user, r);
                await SendAtTimeAsync(command, r.Time);
            });
            await Task.WhenAll(tasks);
        }

        private async Task SendAtTimeAsync(SendReminderCommand command, DateTime time)
        {
            var delay = time - DateTime.Now;
            if (delay.TotalMilliseconds > 0)
            {
                await Task.Delay(delay);
            }

            using var scope = _scopeFactory.CreateScope();
            var commandDispatcher = scope.ServiceProvider.GetRequiredService<IMessageDispatcher>();

            await commandDispatcher.DispatchAsync(command);
        }

        private async Task<List<ReminderModel>> GetRemindersForAppUser(AppUserDto appUserDto)
        {
            if (appUserDto.Circles == null || appUserDto.Circles.Count == 0)
            {
                return new List<ReminderModel>();
            }

            return await _reminderRepository.FindRemindersToSendAsync(appUserDto.Circles.Select(c => c.Id).ToList(),
                                                                               appUserDto.Id,
                                                                               DateTime.Now.AddMinutes(6));
        }
    }
}