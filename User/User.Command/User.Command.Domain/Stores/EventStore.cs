using Core.Messages;
using Core.Repositories;
using Microsoft.IdentityModel.Tokens;
using User.Command.Application.Repositories;
using User.Common.DAOs;
using User.Common.Events;

namespace User.Command.Domin.Stores
{
    public class EventStore : IEventStore<UserEventModel>
    {
        private readonly EventStoreRepository _eventStoreRepository;

        public EventStore(EventStoreRepository eventStoreRepository)
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
                return new List<BaseEvent>();
            }
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            var events = await _eventStoreRepository.FindByUsername(username); 

            if (events != null && events.Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<List<BaseEvent>> GetByUsernameAsync(string username)
        {
            var events = await _eventStoreRepository.FindByUsername(username);
            if (!events.IsNullOrEmpty())
            {
                return events.OrderBy(e => e.Version)
                            .Select(e => e.Event)
                            .Where(e => e.EventType == typeof(UserCreatedEvent).FullName || e.EventType == typeof(UserEditedEvent).FullName)
                            .ToList();
            }
            else
            {
                return new List<BaseEvent>();
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var events = await _eventStoreRepository.FindByEmail(email); 

            if (events != null && events.Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task SaveEventAsync(UserEventModel eventModel)
        {
            await _eventStoreRepository.SaveAsync(eventModel);
        }
    }
}