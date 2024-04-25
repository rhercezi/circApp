using Core.Events.PublicEvents;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;
using Tasks.Command.Application.Commands;
using Tasks.Command.Application.Events;
using Tasks.Command.Application.Exceptions;
using Tasks.Domain.Repositories;

namespace Tasks.Command.Application.Handlers
{
    public class DeleteTaskCommandHandler : ICommandHandler<DeleteTaskCommand>
    {
        private readonly AppTaskRepository _repository;
        private readonly TasksEventProducer _eventProducer;
        private readonly ILogger<DeleteTaskCommandHandler> _logger;

        public DeleteTaskCommandHandler(AppTaskRepository repository,
                                        TasksEventProducer eventProducer)
        {
            _repository = repository;
            _eventProducer = eventProducer;
        }

        public async Task HandleAsync(DeleteTaskCommand command)
        {
            var model = await _repository.GetTasksById(command.Id);
            _logger.LogDebug("Deleting task: {Task}", model);
            var result = await _repository.DeleteTask(command.Id);
            if (result.DeletedCount == 0)
            {
                _logger.LogError("Count of deleted tasks is 0.");
                throw new AppTaskException("Failed deleting task");
            }
            var taskEvent = new TaskChangePublicEvent(command.Id, command.GetType().Name)
            {
                CircleId = model.CircleId,
                UserIds = model.UserModels?.Select(x => x.Id).ToList()
            };
            await _eventProducer.ProduceAsync(taskEvent);
        }

        public Task HandleAsync(BaseCommand command)
        {
            return HandleAsync((DeleteTaskCommand)command);
        }
    }
}