using Core.Messages;

namespace Core.Events.PublicEvents
{
    public class AppointmentChangePublicEvent : BaseEvent
    {
        public Guid AppointmentId { get; set; }
        public string Title { get; set; }
        public EventType Action { get; set; }
        public Guid UserId { get => Id; set => Id = value; }
        public DateTime Date { get; set; }
        public List<Guid> Circles { get; set; }
        public AppointmentChangePublicEvent(
                                            Guid appointmentId,
                                            string title,
                                            EventType action,
                                            Guid userId,
                                            DateTime date,
                                            List<Guid> circles
                                            ) : base(userId, 0, typeof(AppointmentChangePublicEvent).FullName)
        {
            AppointmentId = appointmentId;
            Title = title;
            Action = action;
            UserId = userId;
            Date = date;
            Circles = circles;
        }
    }
}