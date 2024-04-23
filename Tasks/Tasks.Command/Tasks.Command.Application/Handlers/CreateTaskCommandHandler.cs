using Core.Events.PublicEvents;
using Core.MessageHandling;
using Core.Messages;
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

        public CreateTaskCommandHandler(AppTaskRepository repository,
                                        TasksEventProducer eventProducer)
        {
            _repository = repository;
            _eventProducer = eventProducer;
        }

        public async Task HandleAsync(CreateTaskCommand command)
        {
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