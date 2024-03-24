using Core.Messages;

namespace Appointments.Command.Application.Commands
{
    public class DeleteAppointmentCommand : BaseCommand
    {
        public Guid AppointmentId { get => Id; set => Id = value; }
        public Guid UserId { get; set; }
    }
}