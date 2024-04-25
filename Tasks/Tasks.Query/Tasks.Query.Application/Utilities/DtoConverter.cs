using Core.DTOs;
using Tasks.Domain.Entities;

namespace Tasks.Query.Application.Utilities
{
    public class DtoConverter
    {
        public static TaskDto Convert(AppTaskModel task)
        {
            var users = task.UserModels?.Select(u => new TaskUserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                IsCompleted = u.IsCompleted,
                CompletedAt = u.CompletedAt
            }).ToList();

            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                EndDate = task.EndDate,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                ParentTaskId = task.ParentTaskId,
                CircleId = task.CircleId,
                UserModels = users
            };
        }
    }
}