using Core.Messages;

namespace Core.Events.PublicEvents
{
    public class UserUpdatedPublicEvent : BaseEvent
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string FamilyName { get; set; }
        public string Email { get; set; }

        public UserUpdatedPublicEvent(Guid id,
                                string userName,
                                string firstName,
                                string familyName,
                                string email) : base(id, 0, typeof(UserUpdatedPublicEvent).FullName)
        {
            UserName = userName;
            FirstName = firstName;
            FamilyName = familyName;
            Email = email;
        }
    }
}