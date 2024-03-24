using Core.Messages;

namespace Appointments.Command.Application.Commands
{
    public class UpdateAppointmentCommand: BaseCommand
    {
        public Guid UpdaterId { get; set; }
        public DateTime Date { get; set; }
        public List<Guid>? DetailsInCircles { get; set; }
        public required List<Guid> Circles { get; set; }
    }
}