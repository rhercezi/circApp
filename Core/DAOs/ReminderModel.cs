namespace Core.DAOs
{
    public class ReminderModel
    {
        public ReminderTargetType TargetType { get; set; }
        public Guid TargetId { get; set; }
        public required DateTime Time { get; set; }
        public string? Message { get; set; }
        public bool JustForUser { get; set; }
    }
}