using Core.MessageHandling;
using Core.Messages;
using Tasks.Command.Application.Commands;
using Tasks.Command.Application.Exceptions;
using Tasks.Domain.Repositories;

namespace Tasks.Command.Application.Handlers
{
    public class DeleteTaskCommandHandler : ICommandHandler<DeleteTaskCommand>
    {
        private readonly AppTaskRepository _repository;

        public DeleteTaskCommandHandler(AppTaskRepository repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(DeleteTaskCommand command)
        {
            var result = await _repository.DeleteTask(command.Id);
            if (result.DeletedCount == 0)
            {
                throw new AppTaskException("Failed deleting task");
            }
        }

        public Task HandleAsync(BaseCommand command)
        {
            return HandleAsync((DeleteTaskCommand)command);
        }
    }
}