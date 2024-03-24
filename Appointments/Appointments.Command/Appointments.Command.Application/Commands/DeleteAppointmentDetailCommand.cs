using Core.Messages;

namespace Appointments.Command.Application.Commands
{
    public class DeleteAppointmentDetailCommand : BaseCommand
    {
        public Guid UserId { get => Id; set => Id = value; }
        public Guid AppointmentId { get; set; }
    }
}