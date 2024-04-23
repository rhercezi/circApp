using Core.MessageHandling;
using Core.Messages;
using MongoDB.Driver;
using Tasks.Command.Application.Commands;
using Tasks.Command.Application.Exceptions;
using Tasks.Domain.Repositories;

namespace Tasks.Command.Application.Handlers
{
    public class CompleteTaskCommandHandler : ICommandHandler<CompleteTaskCommand>
    {
        private readonly AppTaskRepository _repository;

        public CompleteTaskCommandHandler(AppTaskRepository repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(CompleteTaskCommand command)
        {
            ReplaceOneResult? result = null;
            var model = await _repository.GetTasksById(command.Id);
            if (model.CircleId != null && model.CircleId == command.CircleId)
            {
                model.IsCompleted = true;
                result = await _repository.UpdateTask(model);
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