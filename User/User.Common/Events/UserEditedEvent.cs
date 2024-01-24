using System.Text.Json.Serialization;
using Core.Messages;

namespace User.Common.Events
{
    public class UserEditedEvent : BaseEvent
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string FamilyName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime Updated { get; set; }

        [JsonConstructor]
        public UserEditedEvent(Guid id,
                                  int version,
                                  string userName,
                                  string firstName,
                                  string familyName,
                                  string email,
                                  bool emailConfirmed,
                                  DateTime updated) : base(id, version, typeof(UserEditedEvent).FullName)
        {
            UserName = userName;
            FirstName = firstName;
            FamilyName = familyName;
            Email = email;
            EmailConfirmed = emailConfirmed;
            Updated = updated;
        }
    }
}