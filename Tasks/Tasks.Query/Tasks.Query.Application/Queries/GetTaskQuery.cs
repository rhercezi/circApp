using Core.Messages;

namespace Tasks.Query.Application.Queries
{
    public class GetTaskQuery : BaseQuery
    {
        public Guid TaskId { get; set; }
    }
}