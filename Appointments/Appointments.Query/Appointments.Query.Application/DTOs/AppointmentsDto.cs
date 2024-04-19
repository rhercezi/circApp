using Core.DTOs;

namespace Appointments.Query.Application.DTOs
{
    public class AppointmentsDto : BaseDto
    {
        public List<AppointmentDto>? Appointments { get; set; }
    }
}