using Core.Messages;

namespace Appointments.Query.Application.Queries
{
    public class GetAppointmentsByUserIdQuery : BaseQuery
    {
        public Guid UserId { get; set; }
    }
}