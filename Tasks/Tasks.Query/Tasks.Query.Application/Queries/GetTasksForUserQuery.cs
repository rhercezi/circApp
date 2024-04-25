using Core.Messages;

namespace Tasks.Query.Application.Queries
{
    public class GetTasksForUserQuery : BaseQuery
    {
        public Guid UserId { get; set; }
        public bool SearchByCircles { get; set; }
    }
}