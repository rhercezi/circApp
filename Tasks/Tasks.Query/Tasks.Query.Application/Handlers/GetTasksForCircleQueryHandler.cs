using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;
using Tasks.Domain.Repositories;
using Tasks.Query.Application.Queries;
using Tasks.Query.Application.Utilities;

namespace Tasks.Query.Application.Handlers
{
    public class GetTasksForCircleQueryHandler : IQueryHandler<GetTasksForCircleQuery, TasksDto>
    {
        private readonly AppTaskRepository _taskRepository;
        private readonly ILogger<GetTasksForCircleQueryHandler> _logger;

        public GetTasksForCircleQueryHandler(AppTaskRepository taskRepository,
                                             ILogger<GetTasksForCircleQueryHandler> logger)
        {
            _taskRepository = taskRepository;
            _logger = logger;
        }

        public async Task<TasksDto> HandleAsync(GetTasksForCircleQuery query)
        {
            var tasks = await _taskRepository.GetTasksByCircleId(query.CircleId);
            _logger.LogDebug("Found {Nr} tasks for id {Id}", tasks.Count, query.CircleId);
            return new TasksDto { Tasks = tasks.Select(DtoConverter.Convert).ToList() };
        }

        public async Task<BaseDto> HandleAsync(BaseQuery query)
        {
            return await HandleAsync((GetTasksForCircleQuery)query);
        }
    }
}