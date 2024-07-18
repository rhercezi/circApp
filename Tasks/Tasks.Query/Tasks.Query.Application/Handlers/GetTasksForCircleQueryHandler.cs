using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;
using Tasks.Domain.Repositories;
using Tasks.Query.Application.Queries;

namespace Tasks.Query.Application.Handlers
{
    public class GetTasksForCircleQueryHandler : IMessageHandler<GetTasksForCircleQuery>
    {
        private readonly AppTaskRepository _taskRepository;
        private readonly ILogger<GetTasksForCircleQueryHandler> _logger;

        public GetTasksForCircleQueryHandler(AppTaskRepository taskRepository,
                                             ILogger<GetTasksForCircleQueryHandler> logger)
        {
            _taskRepository = taskRepository;
            _logger = logger;
        }

        public async Task<BaseResponse> HandleAsync(GetTasksForCircleQuery query)
        {
            try
            {
                var tasks = await _taskRepository.GetTasksByCircleId(query.CircleId);
                _logger.LogDebug("Found {Nr} tasks for id {Id}", tasks.Count, query.CircleId);
                
                return new BaseResponse { ResponseCode = 200, Data = tasks };
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return new BaseResponse { ResponseCode = 500, Data = "Something went wrong, please try again later." };
            }
        }

        public async Task<BaseResponse> HandleAsync(BaseMessage query)
        {
            return await HandleAsync((GetTasksForCircleQuery)query);
        }
    }
}