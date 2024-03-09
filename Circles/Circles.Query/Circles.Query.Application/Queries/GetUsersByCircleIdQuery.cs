using Core.Messages;

namespace Circles.Query.Application.Queries
{
    public class GetUsersByCircleIdQuery : BaseQuery
    {
        public Guid CircleId { get; set; }
    }
}