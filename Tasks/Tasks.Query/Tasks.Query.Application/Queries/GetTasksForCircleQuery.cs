using Core.Messages;

namespace Tasks.Query.Application.Queries
{
    public class GetTasksForCircleQuery : BaseQuery
    {
        public Guid CircleId { get; set; }
        public bool IncludeCompleted { get; set; }
    }
}