namespace Core.Messages
{
    public abstract class BaseEvent : BaseMessage
    {
        public Guid Id { get; set; }
        public int Version { get; set; }
        public string EventType { get; set; }

        protected BaseEvent(Guid id, int version, string eventType)
        {
            Id = id;
            Version = version;
            EventType = eventType;
        }
    }
}