using Core.Messages;
using Microsoft.AspNetCore.JsonPatch;

namespace Appointments.Command.Application.Commands
{
    public class UpdateAppointmentDetailCommand : BaseCommand
    {
        public Guid UserId { get => Id; set => Id = value; }
        public Guid AppointmentId { get; set; }
        public JsonPatchDocument PatchDocument { get; set; }

        public UpdateAppointmentDetailCommand(Guid userId, Guid appointmentId, JsonPatchDocument patchDocument)
        {
            AppointmentId = appointmentId;
            UserId = userId;
            PatchDocument = patchDocument;
        }
    }
}