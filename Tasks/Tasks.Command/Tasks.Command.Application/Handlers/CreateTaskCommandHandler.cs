using Core.Events.PublicEvents;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;
using Tasks.Command.Application.Commands;
using Tasks.Command.Application.Events;
using Tasks.Command.Application.Utilities;
using Tasks.Domain.Entities;
using Tasks.Domain.Repositories;

namespace Tasks.Command.Application.Handlers
{
    public class CreateTaskCommandHandler : ICommandHandler<CreateTaskCommand>
    {
        private readonly AppTaskRepository _repository;
        private readonly TasksEventProducer _eventProducer;
        private readonly ILogger<CreateTaskCommandHandler> _logger;

        public CreateTaskCommandHandler(AppTaskRepository repository,
                                        TasksEventProducer eventProducer,
                                        ILogger<CreateTaskCommandHandler> logger)
        {
            _repository = repository;
            _eventProducer = eventProducer;
            _logger = logger;
        }

        public async Task HandleAsync(CreateTaskCommand command)
        {
            _logger.LogDebug("Creating task: {Task}", command);
            await _repository.SaveAsync(CommandModelConverter.ConvertToModel<AppTaskModel>(command));
            var taskEvent = new TaskChangePublicEvent(command.Id, command.GetType().Name)
            {
                CircleId = command.CircleId,
                UserIds = command.UserModels?.Select(x => x.Id).ToList()
            };
            await _eventProducer.ProduceAsync(taskEvent);
        }

        public Task HandleAsync(BaseCommand command)
        {
            return HandleAsync((CreateTaskCommand)command);
        }
    }
}