using Core.DAOs;
using Core.Messages;
using Core.Repositories;
using User.Command.Domain.Repositories;

namespace User.Command.Domin.Stores
{
    public class EventStore : IEventStore
    {
        private readonly IEventStoreRepository _eventStoreRepository;

        public EventStore(IEventStoreRepository eventStoreRepository)
        {
            _eventStoreRepository = eventStoreRepository;
        }

        public async Task<List<BaseEvent>> GetEventsAsync(Guid id)
        {
            var events = await _eventStoreRepository.FindByAgregateId(id); 

            if (events != null && events.Any())
            {
                return events.OrderBy(e => e.Version).Select(e => e.Event).ToList();
            }
            else
            {
                throw new ArgumentException("No users found with the given ID.");
            }
        }

        public async Task SaveEventAsync(EventModel eventModel)
        {
            await _eventStoreRepository.SaveAsync(eventModel);
        }
    }
}