using Core.DTOs;
using Core.Events.PublicEvents;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Tasks.Command.Application.Commands;
using Tasks.Command.Application.Events;
using Tasks.Domain.Repositories;

namespace Tasks.Command.Application.Handlers
{
    public class CompleteTaskCommandHandler : IMessageHandler<CompleteTaskCommand>
    {
        private readonly AppTaskRepository _repository;
        private readonly TasksEventProducer _eventProducer;
        private readonly ILogger<CompleteTaskCommandHandler> _logger;

        public CompleteTaskCommandHandler(AppTaskRepository repository,
                                          TasksEventProducer eventProducer,
                                          ILogger<CompleteTaskCommandHandler> logger)
        {
            _repository = repository;
            _eventProducer = eventProducer;
            _logger = logger;
        }

        public async Task<BaseResponse> HandleAsync(CompleteTaskCommand command)
        {
            ReplaceOneResult? result = null;
            var model = await _repository.GetTasksById(command.TaskId);
            _logger.LogDebug("For id '{id}' found task: {task}", command.TaskId.ToString(), model);

            if (model.CircleId != null && model.CircleId == command.CircleId)
            {
                model.IsCompleted = true;
                result = await _repository.UpdateTask(model);

                var taskEvent = new TaskChangePublicEvent(model.Id, command.GetType().Name)
                {
                    CircleId = model.CircleId
                };
                await _eventProducer.ProduceAsync(taskEvent);
            }
            else if (model.UserModels != null && model.UserModels.Any())
            {
                var user = model.UserModels.FirstOrDefault(x => x.Id == command.UserId);
                if (user != null)
                {
                    user.IsCompleted = true;
                    user.CompletedAt = DateTime.UtcNow;
                    result = await _repository.UpdateTask(model);
                }
                //complete task if all users have completed it
                if (model.UserModels.All(x => x.IsCompleted))
                {
                    model.IsCompleted = true;
                    result = await _repository.UpdateTask(model);
                }
                var taskEvent = new TaskChangePublicEvent(model.Id, command.GetType().Name)
                {
                    UserIds = model.UserModels.Select(x => x.Id).ToList()
                };
                await _eventProducer.ProduceAsync(taskEvent);
            }
            else
            {
                return new BaseResponse { ResponseCode = 400, Data = "Task not found" };
            }

            if (result?.ModifiedCount == 0)
            {
                _logger.LogError("Count of modified documents is 0");
                return new BaseResponse { ResponseCode = 500, Data = "Something went wrong, please try again later." };
            }

            return new BaseResponse { ResponseCode = 200, Data = model };
        }

        public async Task<BaseResponse> HandleAsync(BaseMessage command)
        {
            return await HandleAsync((CompleteTaskCommand)command);
        }
    }
}