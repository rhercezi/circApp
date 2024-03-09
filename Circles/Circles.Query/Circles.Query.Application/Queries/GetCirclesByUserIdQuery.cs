using Core.Messages;

namespace Circles.Query.Application.Queries
{
    public class GetCirclesByUserIdQuery : BaseQuery
    {
        public Guid UserId { get; set; }
    }
}