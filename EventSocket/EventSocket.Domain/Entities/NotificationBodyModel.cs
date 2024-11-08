namespace EventSocket.Domain.Entities
{
    public class NotificationBodyModel
    {
        public Guid TargetId { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }
    }
}