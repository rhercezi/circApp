using Core.Messages;

namespace User.Query.Application.Queries
{
    public class GetUserByIdQuery : BaseQuery
    {
        public Guid Id { get; set; }
    }
}