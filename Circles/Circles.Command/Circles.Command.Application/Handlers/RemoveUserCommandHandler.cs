using Circles.Command.Application.Commands;
using Circles.Domain.Repositories;
using Core.MessageHandling;
using Core.Messages;

namespace Circles.Command.Application.Handlers
{
    public class RemoveUserCommandHandler : ICommandHandler<RemoveUsersCommand>
    {
        public UserCircleRepository _userCircleRepository { get; set; }
        public CirclesRepository _circlesRepository { get; set; }
        public RemoveUserCommandHandler(UserCircleRepository userCircleRepository,
                                        CirclesRepository circlesRepository)
        {
            _userCircleRepository = userCircleRepository;
            _circlesRepository = circlesRepository;
        }

        public async Task HandleAsync(RemoveUsersCommand command)
        {
            var deleteTask = command.Users.Select(
                uId => Task.Run(() => _userCircleRepository.DeleteByUserAndCircle(uId, command.CircleId))
            );
            await Task.WhenAll(deleteTask);

            var users = await _userCircleRepository.FindAsync(uc => uc.CircleId == command.CircleId);
            if (users.Count == 0)
            {
                await _circlesRepository.DeleteCircle(command.CircleId);
            }
        }

        public async Task HandleAsync(BaseCommand command)
        {
            await HandleAsync((RemoveUsersCommand)command);
        }
    }
}