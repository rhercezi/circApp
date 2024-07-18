using Core.DTOs;
using Core.Events.PublicEvents;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;
using Tasks.Command.Application.Commands;
using Tasks.Command.Application.Events;
using Tasks.Domain.Repositories;

namespace Tasks.Command.Application.Handlers
{
    public class DeleteTaskCommandHandler : IMessageHandler<DeleteTaskCommand>
    {
        private readonly AppTaskRepository _repository;
        private readonly TasksEventProducer _eventProducer;
        private readonly ILogger<DeleteTaskCommandHandler> _logger;

        public DeleteTaskCommandHandler(AppTaskRepository repository,
                                        TasksEventProducer eventProducer,
                                        ILogger<DeleteTaskCommandHandler> logger)
        {
            _repository = repository;
            _eventProducer = eventProducer;
            _logger = logger;
        }

        public async Task<BaseResponse> HandleAsync(DeleteTaskCommand command)
        {
            try
            {
                var model = await _repository.GetTasksById(command.Id);
                _logger.LogDebug("Deleting task: {Task}", model);

                var result = await _repository.DeleteTask(command.Id);

                if (result.DeletedCount == 0)
                {
                    _logger.LogError("Count of deleted tasks is 0.");
                    return new BaseResponse { ResponseCode = 404, Data = "Task not found." };
                }
                var taskEvent = new TaskChangePublicEvent(command.Id, command.GetType().Name)
                {
                    CircleId = model.CircleId,
                    UserIds = model.UserModels?.Select(x => x.Id).ToList()
                };

                await _eventProducer.ProduceAsync(taskEvent);

                return new BaseResponse { ResponseCode = 204 };
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return new BaseResponse { ResponseCode = 500, Data = "Something went wrong, please try again later." };
            }
        }

        public Task<BaseResponse> HandleAsync(BaseMessage command)
        {
            return HandleAsync((DeleteTaskCommand)command);
        }
    }
}