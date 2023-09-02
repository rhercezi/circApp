using Core.DAOs;
using Core.Messages;

namespace Core.Repositories
{
    public interface IEventStore
    {
        Task SaveEventAsync(EventModel eventModel);
        Task<List<BaseEvent>> GetEventsAsync(Guid id);
    }
}