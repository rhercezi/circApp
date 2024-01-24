using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using User.Query.Domain.Repositories;

namespace User.Query.Application.EventConsuming
{
    public class EventHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public EventHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {

            using (var scope = _serviceProvider.CreateAsyncScope())
            {
                var consumer = scope.ServiceProvider.GetRequiredService<EventConsumer>();
                var repository = scope.ServiceProvider.GetRequiredService<UserRepository>();

                Task.Run(() => consumer.Consume(repository), cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}