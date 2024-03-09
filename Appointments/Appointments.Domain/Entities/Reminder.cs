namespace Appointments.Domain.Entities
{
    public class Reminder
    {
        public DateTime Time { get; set; }
        public string? Message { get; set; }
    }
}