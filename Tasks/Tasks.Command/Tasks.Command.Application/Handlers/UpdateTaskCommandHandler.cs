using Core.DTOs;
using Core.Events.PublicEvents;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;
using Tasks.Command.Application.Commands;
using Tasks.Command.Application.Events;
using Tasks.Command.Application.Exceptions;
using Tasks.Command.Application.Utilities;
using Tasks.Domain.Entities;
using Tasks.Domain.Repositories;

namespace Tasks.Command.Application.Handlers
{
    public class UpdateTaskCommandHandler : IMessageHandler<UpdateTaskCommand>
    {
        private readonly AppTaskRepository _repository;
        private readonly TasksEventProducer _eventProducer;
        private readonly ILogger<UpdateTaskCommandHandler> _logger;

        public UpdateTaskCommandHandler(AppTaskRepository repository,
                                        TasksEventProducer eventProducer,
                                        ILogger<UpdateTaskCommandHandler> logger)
        {
            _repository = repository;
            _eventProducer = eventProducer;
            _logger = logger;
        }

        public async Task<BaseResponse> HandleAsync(UpdateTaskCommand command)
        {
            try
            {
                var modelOld = await _repository.GetTaskById(command.Id);
                var modelNew = CommandModelConverter.ConvertToModel<AppTaskModel>(command);

                var result = await _repository.UpdateTask(modelNew);

                if (result.ModifiedCount == 0)
                {
                    _logger.LogError("Count of updated tasks is 0. Command id: {CommandId}", command.Id.ToString());
                    throw new AppTaskException("Failed updating task");
                }

                var taskEvent = new TaskChangePublicEvent(command.Id, modelNew.Title, EventType.Update, modelNew.EndDate)
                {
                    InitiatorId = command.OwnerId,
                    CircleId = modelNew.CircleId,
                    UserIds = modelNew.UserModels?.Select(x => x.Id).ToList()
                };

                await _eventProducer.ProduceAsync(taskEvent);

                return new BaseResponse { ResponseCode = 200, Data = new { taskOld = modelOld, taskNew = modelNew } };
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return new BaseResponse { ResponseCode = 500, Data = "Something went wrong, please try again later." };
            }
        }

        public Task<BaseResponse> HandleAsync(BaseMessage command)
        {
            return HandleAsync((UpdateTaskCommand)command);
        }
    }
}