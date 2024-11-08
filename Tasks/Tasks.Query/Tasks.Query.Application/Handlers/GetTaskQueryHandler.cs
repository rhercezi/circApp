using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;
using Tasks.Domain.Repositories;
using Tasks.Query.Application.Queries;

namespace Tasks.Query.Application.Handlers
{
    public class GetTaskQueryHandler : IMessageHandler<GetTaskQuery>
    {
        private readonly AppTaskRepository _taskRepository;
        private readonly ILogger<GetTaskQueryHandler> _logger;

        public GetTaskQueryHandler(AppTaskRepository taskRepository, ILogger<GetTaskQueryHandler> logger)
        {
            _taskRepository = taskRepository;
            _logger = logger;
        }

        public async Task<BaseResponse> HandleAsync(GetTaskQuery message)
        {
            try
            {
                var task = await _taskRepository.GetTaskById(message.TaskId);
                _logger.LogDebug("Found task with id {Id}", message.TaskId);
                
                return new BaseResponse { ResponseCode = 200, Data = task };
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return new BaseResponse { ResponseCode = 500, Data = "Something went wrong, please try again later." };
            }
        }

        public Task<BaseResponse> HandleAsync(BaseMessage message)
        {
            return HandleAsync((GetTaskQuery)message);
        }
    }
}