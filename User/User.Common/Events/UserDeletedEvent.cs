using System.Text.Json.Serialization;
using Core.Messages;

namespace User.Common.Events
{
    public class UserDeletedEvent : BaseEvent
    {
        [JsonConstructor]
        public UserDeletedEvent(Guid id,
                                int version) : base(id, version, typeof(UserDeletedEvent).FullName)
        {
        }
    }
}