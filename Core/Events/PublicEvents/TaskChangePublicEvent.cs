using Core.Messages;

namespace Core.Events.PublicEvents
{
    public class TaskChangePublicEvent : BaseEvent
    {
        
        public Guid TaskId { get => Id; set => Id = value; }
        public Guid? CircleId { get; set; }
        public Guid? InitiatorId { get; set; }
        public List<Guid>? UserIds { get; set; }
        public string Title { get; set; }
        public EventType Action { get; set; }
        public DateTime Date { get; set; }
        public TaskChangePublicEvent(Guid taskId, string title, EventType action, DateTime date) : base(taskId, 0, typeof(TaskChangePublicEvent).FullName)
        {
            Title = title;
            Action = action;
            Date = date;
        }
    }
}