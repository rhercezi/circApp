using Core.DAOs;
using Core.Messages;

namespace Core.Repositories
{
    public interface IEventStore<T> where T : EventModel
    {
        public Task SaveEventAsync(T eventModel);
        public Task<List<BaseEvent>> GetEventsAsync(Guid id);
    }
}