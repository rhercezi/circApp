namespace EventSocket.Application.DTOs
{
    public class ReminderDto
    {
        public Guid AppointmentId { get; set; }
        public Guid CreatorId { get; set; }
        public required DateTime Time { get; set; }
        public string? Message { get; set; }
        public bool JustForUser { get; set; }
    }
}