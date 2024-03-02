using Core.Events;
using Core.MessageHandling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Circles.Command.Application.EventConsumer
{
    public class EventHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EventHostedService> _logger;

        public EventHostedService(IServiceProvider serviceProvider, ILogger<EventHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateAsyncScope();
                var consumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();
                var eventDispatcher = scope.ServiceProvider.GetRequiredService<IEventDispatcher>();

                Task.Run(() => consumer.Consume(eventDispatcher), cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e.StackTrace, e.Message);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}