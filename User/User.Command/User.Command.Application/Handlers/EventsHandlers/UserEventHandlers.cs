using Core.Aggregate;
using Core.MessageHandling;
using User.Command.Domain.Aggregates;

namespace User.Command.Application.Handlers.EventsHandlers
{
    public class UserEventHandlers : IEventHandler<UserAggregate>
    {
        public Task<UserAggregate> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task SaveAsync(AbstractAggregate aggregate)
        {
            throw new NotImplementedException();
        }
    }
}