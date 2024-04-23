using Core.Events.PublicEvents;
using Core.MessageHandling;
using Core.Messages;
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

        public UpdateTaskCommandHandler(AppTaskRepository repository,
                                        TasksEventProducer eventProducer)
        {
            _repository = repository;
            _eventProducer = eventProducer;
        }

        public async Task HandleAsync(UpdateTaskCommand command)
        {
            var result = await _repository.UpdateTask(CommandModelConverter.ConvertToModel<AppTaskModel>(command));
            if (result.ModifiedCount == 0)
            {
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