using Core.Messages;

namespace Core.Events.PublicEvents
{
    public class TaskChangePublicEvent : BaseEvent
    {
        
        public Guid TaskId { get => Id; set => Id = value; }
        public Guid? CircleId { get; set; }
        public List<Guid>? UserIds { get; set; }
        public string ActionType { get; set; }
        public TaskChangePublicEvent(Guid taskId, string actionType) : base(taskId, 0, typeof(TaskChangePublicEvent).FullName)
        {
            ActionType = actionType;
        }
    }
}