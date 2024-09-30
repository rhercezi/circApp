using Core.Messages;

namespace Appointments.Query.Application.Queries
{
    public class GetAppointmentByIdQuery : BaseQuery
    {
        public Guid Id { get; set; }
    }
}