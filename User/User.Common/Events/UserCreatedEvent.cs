using System.Text.Json.Serialization;
using Core.Messages;

namespace User.Common.Events
{
    public class UserCreatedEvent : BaseEvent
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string FamilyName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime Created { get; set; }

        [JsonConstructor]
        public UserCreatedEvent(Guid id,
                                   int version,
                                   string userName,
                                   string password,
                                   string firstName,
                                   string familyName,
                                   string email,
                                   bool emailConfirmed,
                                   DateTime created) : base(id, version, typeof(UserCreatedEvent).FullName)
        {
            UserName = userName;
            Password = password;
            FirstName = firstName;
            FamilyName = familyName;
            Email = email;
            EmailConfirmed = emailConfirmed;
            Created = created;
        }
    }
}