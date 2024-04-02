namespace Appointments.Domain.Entities
{
    public class Reminder
    {
        public required DateTime Time { get; set; }
        public string? Message { get; set; }
    }
}