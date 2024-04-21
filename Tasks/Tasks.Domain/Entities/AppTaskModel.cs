namespace Tasks.Domain.Entities
{
    public class AppTaskModel
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid? ParentTaskId { get; set; }
        public List<TaskUserModel>? UserModels { get; set; }
        public Guid? CircleId { get; set; }

    }
}   