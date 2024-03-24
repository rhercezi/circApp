using Appointments.Command.Application.DTOs;
using Core.Messages;

namespace Appointments.Command.Application.Commands
{
    public class AddAppointmentDetailsCommand : BaseCommand
    {
        public Guid UserId { get => Id; set => Id = value; }
        public AppointmentDetailsDto? Details { get; set; }
    }
}