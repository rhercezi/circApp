using Core.MessageHandling;
using Core.Messages;
using Tasks.Command.Application.Commands;
using Tasks.Command.Application.Utilities;
using Tasks.Domain.Entities;
using Tasks.Domain.Repositories;

namespace Tasks.Command.Application.Handlers
{
    public class CreateTaskCommandHandler : ICommandHandler<CreateTaskCommand>
    {
        private readonly AppTaskRepository _repository;

        public CreateTaskCommandHandler(AppTaskRepository repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(CreateTaskCommand command)
        {
            await _repository.SaveAsync(CommandModelConverter.ConvertToModel<AppTaskModel>(command));
        }

        public Task HandleAsync(BaseCommand command)
        {
            return HandleAsync((CreateTaskCommand)command);
        }
    }
}