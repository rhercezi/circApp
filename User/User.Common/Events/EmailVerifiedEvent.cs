using Core.Messages;

namespace User.Common.Events
{
    public class EmailVerifiedEvent : BaseEvent
    {
        public EmailVerifiedEvent(Guid id, int version) : base(id, version, typeof(EmailVerifiedEvent).FullName)
        {
        }
    }
}