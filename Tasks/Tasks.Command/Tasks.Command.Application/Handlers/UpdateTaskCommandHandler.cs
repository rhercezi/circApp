using Core.MessageHandling;
using Core.Messages;
using Tasks.Command.Application.Commands;
using Tasks.Command.Application.Exceptions;
using Tasks.Command.Application.Utilities;
using Tasks.Domain.Entities;
using Tasks.Domain.Repositories;

namespace Tasks.Command.Application.Handlers
{
    public class UpdateTaskCommandHandler : ICommandHandler<UpdateTaskCommand>
    {
        private readonly AppTaskRepository _repository;

        public UpdateTaskCommandHandler(AppTaskRepository repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(UpdateTaskCommand command)
        {
            var result = await _repository.UpdateTask(CommandModelConverter.ConvertToModel<AppTaskModel>(command));
            if (result.ModifiedCount == 0)
            {
                throw new AppTaskException("Failed updating task");
            }
        }

        public Task HandleAsync(BaseCommand command)
        {
            return HandleAsync((UpdateTaskCommand)command);
        }
    }
}