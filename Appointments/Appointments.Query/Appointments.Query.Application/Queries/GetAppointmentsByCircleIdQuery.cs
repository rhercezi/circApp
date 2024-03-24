using Core.Messages;

namespace Appointments.Query.Application.Queries
{
    public class GetAppointmentsByCircleIdQuery : BaseQuery
    {
        public Guid CircleId { get; set; }
    }
}