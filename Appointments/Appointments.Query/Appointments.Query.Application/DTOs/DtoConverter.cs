using Appointments.Domain.Entities;

namespace Appointments.Query.Application.DTOs
{
    public static class DtoConverter
    {
        public static AppointmentDetailsDto Convert(AppointmentDetailsModel model)
        {
            return new AppointmentDetailsDto
            {
                AppointmentId = model.AppointmentId,
                Note = model.Note,
                Address = model.Address,
                Reminders = model.Reminders
            };
        }

        internal static AppointmentDto Convert(AppointmentModel model)
        {
            return new AppointmentDto
            {
                Id = model.Id,
                CreatorId = model.CreatorId,
                Date = model.StartDate,
                DetailsInCircles = model.DetailsInCircles,
                Details = model.Details != null ? Convert(model.Details) : null
            };
        }
    }
}