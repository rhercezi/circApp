using Circles.Command.Application.Commands;
using Circles.Domain.Entities;
using Circles.Domain.Repositories;
using Core.MessageHandling;
using Core.Messages;

namespace Circles.Command.Application.Handlers
{
    public class UpdateCircleCommandHandler : ICommandHandler<UpdateCircleCommand>
    {
        public CirclesRepository _circlesRepository { get; set; }

        public UpdateCircleCommandHandler(CirclesRepository circlesRepository)
        {
            _circlesRepository = circlesRepository;
        }

        public async Task HandleAsync(UpdateCircleCommand command)
        {
            await _circlesRepository.UpdateCirce(
                new CircleModel
                {
                    CircleId = command.CircleId,
                    Name = command.Name,
                    Color = command.Color
                }
            );
        }

        public async Task HandleAsync(BaseCommand command)
        {
            await HandleAsync((UpdateCircleCommand)command);
        }
    }
}