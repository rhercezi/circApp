using Core.Messages;
using Tasks.Domain.Entities;

namespace Tasks.Command.Application.Commands
{
    public class CreateTaskCommand : BaseCommand
    {
        public Guid OwnerId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? ParentTaskId { get; set; }
        public List<TaskUserModel>? UserModels { get; set; }
        public Guid? CircleId { get; set; }
    }
}