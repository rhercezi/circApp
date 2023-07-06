using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Messages;
using Core.Repositories;
using User.Command.Domain.Repositories;

namespace User.Command.Application.Stores
{
    public class EventStore : IEventStore
    {
        private readonly IEventStoreRepository _eventStoreRepository;

        public EventStore(IEventStoreRepository eventStoreRepository)
        {
            _eventStoreRepository = eventStoreRepository;
        }

        public Task<List<BaseEvent>> GetEventsAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task SaveEvetnAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int version)
        {
            throw new NotImplementedException();
        }
    }
}