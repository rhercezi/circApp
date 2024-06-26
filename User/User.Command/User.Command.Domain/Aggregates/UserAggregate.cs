using Core.Aggregate;
using Core.Messages;
using User.Command.Domain.Exceptions;
using User.Command.Domin.Stores;

namespace User.Command.Domain.Aggregates
{
    public class UserAggregate : AbstractAggregate
    {
        public readonly EventStore _eventStore;

        public string? UserName { get; set; }
        public string? Email { get; set; }

        public UserAggregate(EventStore eventStore)
        {
            _eventStore = eventStore;
        }
        
        public void InvokAction<T>(BaseEvent xEvent) where T : BaseEvent
        {
            if (_isDeleted) throw new UserDomainException("Can not perform action on deleted user");
            InvokeAction<T,UserAggregate>(xEvent, this, false);
        }
        public void ReplayEvents(List<BaseEvent> events)
        {
            this.ReplayEvents(events,this, true);
        }

        public void RestoreReadDb(List<BaseEvent> events)
        {
            this.ReplayEvents(events,this, false);
        }
    }
}