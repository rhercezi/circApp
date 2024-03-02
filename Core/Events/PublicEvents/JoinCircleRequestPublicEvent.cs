using Core.Messages;

namespace Core.Events.PublicEvents
{
    public class JoinCircleRequestPublicEvent : BaseEvent
    {
        public Guid CircleId { get; set; }
        public Guid UserId { get => Id; set => Id = value; }
        public Guid InviterId { get; set; }
        public JoinCircleRequestPublicEvent(
                                      Guid circleId,
                                      Guid userId,
                                      Guid inviterId) : base(userId, 0, typeof(JoinCircleRequestPublicEvent).FullName)
        {
            CircleId = circleId;
            UserId = userId;
            InviterId = inviterId;
        }
    }
}