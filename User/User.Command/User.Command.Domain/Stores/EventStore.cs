using Core.Messages;
using Core.Repositories;
using User.Command.Application.Repositories;
using User.Command.Domain.Exceptions;
using User.Common.DAOs;

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
                throw new UserDomainException("No users found with the given ID.");
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