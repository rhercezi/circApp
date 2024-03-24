using Appointments.Domain.Entities;

namespace Appointments.Query.Application.DTOs
{
    public class AppointmentDetailsDto
    {
        public Guid AppointmentId { get; set; }
        public string? Note { get; set; }
        public Address? Address { get; set; }
        public List<Reminder>? Reminders { get; set; }
    }
}