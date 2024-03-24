using Appointments.Command.Application.Commands;
using Appointments.Domain.Entities;

namespace Appointments.Command.Application.DTOs
{
    public static class DtoConverter
    {
        public static AppointmentModel Convert(CreateAppointmentCommand command)
        {
            return new AppointmentModel
            {
                Id = command.Id,
                CreatorId = command.CreatorId,
                Date = command.Date,
                DetailsInCircles = command.Circles
            };
        }

        public static AppointmentDetailsModel Convert(AppointmentDetailsDto details)
        {
            return new AppointmentDetailsModel
            {
                AppointmentId = details.AppointmentId,
                Note = details.Note,
                Address = details.Address,
                Reminders = details.Reminders
            };
        }

        internal static AppointmentModel Convert(UpdateAppointmentCommand command)
        {
            return new AppointmentModel
            {
                Id = command.Id,
                CreatorId = command.UpdaterId,
                Date = command.Date,
                DetailsInCircles = command.Circles
            };
        }

        internal static AppointmentDetailsModel Convert(UpdateAppointmentDetailCommand command)
        {
            return new AppointmentDetailsModel
            {
                AppointmentId = command.Details.AppointmentId,
                Note = command.Details.Note,
                Address = command.Details.Address,
                Reminders = command.Details.Reminders
            };
        }
    }
}