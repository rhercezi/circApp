using Core.Messages;

namespace Core.Events.PublicEvents
{
    public class UserDeletedPublicEvent : BaseEvent
    {
        public UserDeletedPublicEvent(Guid id) : base(id, 0, typeof(UserDeletedPublicEvent).FullName)
        {
        }
    }
}