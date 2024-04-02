using Core.Messages;

namespace Appointments.Query.Application.Queries
{
    public class GetAppointmentsByCircleIdQuery : BaseQuery
    {
        public required Guid CircleId { get; set; }
        public required DateTime DareFrom { get; set; }
        public required DateTime DateTo { get; set; }
    }
}