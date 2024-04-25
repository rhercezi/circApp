namespace Core.DTOs
{
    public class TasksDto : BaseDto
    {
        public List<TaskDto>? Tasks { get; set; }
    }
    public class TaskDto
        {
            public Guid Id { get; set; }
            public required string Title { get; set; }
            public required string Description { get; set; }
            public bool IsCompleted { get; set; }
            public DateTime EndDate { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public Guid? ParentTaskId { get; set; }
            public List<TaskUserDto>? UserModels { get; set; }
            public Guid? CircleId { get; set; }
        }
        public class TaskUserDto
        {
            public Guid Id { get; set; }
            public required string UserName { get; set; }
            public bool IsCompleted { get; set; }
            public DateTime? CompletedAt { get; set; }
        }
}