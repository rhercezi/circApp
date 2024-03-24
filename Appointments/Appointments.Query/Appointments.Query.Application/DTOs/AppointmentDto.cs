using Core.DTOs;

namespace Appointments.Query.Application.DTOs
{
    public class AppointmentDto : BaseDto
    {
        public Guid Id { get; set; }
        public Guid CreatorId { get; set; }
        public DateTime Date { get; set; }
        public AppointmentDetailsDto? Details { get; set; }
        public List<Guid>? DetailsInCircles { get; set; }
    }
}