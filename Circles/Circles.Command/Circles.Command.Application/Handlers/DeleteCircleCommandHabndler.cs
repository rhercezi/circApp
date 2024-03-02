using Circles.Command.Application.Commands;
using Circles.Domain.Repositories;
using Core.MessageHandling;
using Core.Messages;

namespace Circles.Command.Application.Handlers
{
    public class DeleteCircleCommandHabndler : ICommandHandler<DeleteCircleCommand>
    {
        public CirclesRepository _circlesRepository { get; set; }
        public UserCircleRepository _userCircleRepository { get; set; }
        public DeleteCircleCommandHabndler(CirclesRepository circlesRepository, UserCircleRepository userCircleRepository)
        {
            _circlesRepository = circlesRepository;
            _userCircleRepository = userCircleRepository;
        }

        public async Task HandleAsync(DeleteCircleCommand command)
        {
            await _userCircleRepository.DeleteByPredicate(uc => uc.CircleId == command.CircleId);
            await _circlesRepository.DeleteCircle(command.CircleId);
        }

        public async Task HandleAsync(BaseCommand command)
        {
            await HandleAsync((DeleteCircleCommand)command);
        }
    }
}