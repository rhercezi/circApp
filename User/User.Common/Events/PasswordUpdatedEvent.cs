using System.Text.Json.Serialization;
using Core.Messages;

namespace User.Common.Events
{
    public class PasswordUpdatedEvent : BaseEvent
    {
        public string Password { get; set; }
        public DateTime Updated { get; set; }

        [JsonConstructor]
        public PasswordUpdatedEvent(Guid id,
                                    int version,
                                    string password,
                                    DateTime updated) : base(id, version, typeof(PasswordUpdatedEvent).FullName)
        {
            Password = password;
            Updated = updated;
        }
    }
}