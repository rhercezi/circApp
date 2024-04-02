using Core.Messages;

namespace Appointments.Query.Application.Queries
{
    public class GetAppointmentsByUserIdQuery : BaseQuery
    {
        public required Guid UserId { get; set; }
        public required DateTime DateFrom { get; set; }
        public required DateTime DateTo { get; set; }
    }
}