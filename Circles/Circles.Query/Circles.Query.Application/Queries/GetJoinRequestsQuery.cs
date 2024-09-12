using Core.Messages;

namespace Circles.Query.Application.Queries
{
    public class GetJoinRequestsQuery : BaseQuery
    {
        public Guid UserId { get; set; }
    }
}