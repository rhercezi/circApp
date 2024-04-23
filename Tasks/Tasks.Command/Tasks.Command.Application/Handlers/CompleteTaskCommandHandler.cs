using Core.Events.PublicEvents;
using Core.MessageHandling;
using Core.Messages;
using MongoDB.Driver;
using Tasks.Command.Application.Commands;
using Tasks.Command.Application.Events;
using Tasks.Command.Application.Exceptions;
using Tasks.Domain.Repositories;

namespace Tasks.Command.Application.Handlers
{
    public class CompleteTaskCommandHandler : ICommandHandler<CompleteTaskCommand>
    {
        private readonly AppTaskRepository _repository;
        private readonly TasksEventProducer _eventProducer;

        public CompleteTaskCommandHandler(AppTaskRepository repository,
                                          TasksEventProducer eventProducer)
        {
            _repository = repository;
            _eventProducer = eventProducer;
        }

        public async Task HandleAsync(CompleteTaskCommand command)
        {
            ReplaceOneResult? result = null;
            var model = await _repository.GetTasksById(command.Id);
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
                throw new AppTaskException("Task not found");
            }

            if (result?.ModifiedCount == 0)
            {
                throw new AppTaskException("Failed completing task");
            }
        }

        public Task HandleAsync(BaseCommand command)
        {
            return HandleAsync((CompleteTaskCommand)command);
        }
    }
}