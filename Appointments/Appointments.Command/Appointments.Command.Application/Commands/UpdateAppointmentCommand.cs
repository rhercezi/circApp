using Core.Messages;
using Microsoft.AspNetCore.JsonPatch;

namespace Appointments.Command.Application.Commands
{
    public class UpdateAppointmentCommand: BaseCommand
    {
        public UpdateAppointmentCommand(Guid updaterId,
                                        Guid appointmentId,
                                        JsonPatchDocument jsonPatchDocument)
        {
            UpdaterId = updaterId;
            Id = appointmentId;
            JsonPatchDocument = jsonPatchDocument;
        }

        public Guid UpdaterId { get; set; }
        public JsonPatchDocument JsonPatchDocument { get; set; }
    }
}