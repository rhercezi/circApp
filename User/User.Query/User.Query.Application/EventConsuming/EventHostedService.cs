using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using User.Query.Application.Exceptions;
using User.Query.Domain.Repositories;

namespace User.Query.Application.EventConsuming
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
                var consumer = scope.ServiceProvider.GetRequiredService<EventConsumer>();
                var repository = scope.ServiceProvider.GetRequiredService<UserRepository>();

                Task.Run(() => consumer.Consume(repository, _serviceProvider), cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e.StackTrace, e.Message);
                throw new QueryApplicationException("Failed running event consumer." ,e);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}