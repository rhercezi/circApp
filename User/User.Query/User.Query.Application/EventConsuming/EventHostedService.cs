using Core.Events;
using Core.MessageHandling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace User.Query.Application.EventConsuming
{
    public class EventHostedService : BackgroundService
    {
        private readonly ILogger<EventHostedService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public EventHostedService(IServiceScopeFactory scopeFactory, ILogger<EventHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Event consumer started.");
            stoppingToken.Register(() => _logger.LogInformation("Event consumer stopping."));

            Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var consumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();
                        var eventDispatcher = scope.ServiceProvider.GetRequiredService<IMessageDispatcher>();

                        await consumer.Consume(eventDispatcher);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "An exception occurred: {Message}", e.Message);
                    }

                    await Task.Delay(1000, stoppingToken);
                }

            }, stoppingToken);
            _logger.LogInformation("Event consumer stopped.");
            
            return Task.CompletedTask;
        }
    }
}
