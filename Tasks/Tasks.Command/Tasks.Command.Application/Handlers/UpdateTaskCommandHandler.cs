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
    public class UpdateTaskCommandHandler : ICommandHandler<UpdateTaskCommand>
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

        public async Task HandleAsync(UpdateTaskCommand command)
        {
            var result = await _repository.UpdateTask(CommandModelConverter.ConvertToModel<AppTaskModel>(command));
            if (result.ModifiedCount == 0)
            {
                _logger.LogError("Count of updated tasks is 0. Command id: {CommandId}", command.Id.ToString());
                throw new AppTaskException("Failed updating task");
            }
            var taskEvent = new TaskChangePublicEvent(command.Id, command.GetType().Name)
            {
                CircleId = command.CircleId,
                UserIds = command.UserModels?.Select(x => x.Id).ToList()
            };
            await _eventProducer.ProduceAsync(taskEvent);
        }

        public Task HandleAsync(BaseCommand command)
        {
            return HandleAsync((UpdateTaskCommand)command);
        }
    }
}