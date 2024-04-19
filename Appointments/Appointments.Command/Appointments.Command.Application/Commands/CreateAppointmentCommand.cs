using Appointments.Command.Application.DTOs;
using Core.Messages;

namespace Appointments.Command.Application.Commands
{
    public class CreateAppointmentCommand: BaseCommand
    {
        public Guid CreatorId { get; set; }
        public DateTime Date { get; set; }
        public AppointmentDetailsDto? Details { get; set; }
        public List<Guid>? DetailsInCircles { get; set; }
        public required List<Guid> Circles { get; set; }

        public CreateAppointmentCommand()
        {
            Id = Guid.NewGuid();
        }
    }
}