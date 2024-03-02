using Core.Messages;

namespace Core.Events.PublicEvents
{
    public class UserCreatedPublicEvent : BaseEvent
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string FamilyName { get; set; }
        public string Email { get; set; }

        public UserCreatedPublicEvent(Guid id,
                                string userName,
                                string firstName,
                                string familyName,
                                string email) : base(id, 0, typeof(UserCreatedPublicEvent).FullName)
        {
            UserName = userName;
            FirstName = firstName;
            FamilyName = familyName;
            Email = email;
        }
    }
}