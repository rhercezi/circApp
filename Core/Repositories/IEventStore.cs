using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Messages;

namespace Core.Repositories
{
    public interface IEventStore
    {
        Task SaveEvetnAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int version);
        Task<List<BaseEvent>> GetEventsAsync(Guid id);
    }
}