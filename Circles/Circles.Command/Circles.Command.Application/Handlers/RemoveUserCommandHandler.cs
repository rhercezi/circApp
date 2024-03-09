using Circles.Command.Application.Commands;
using Circles.Domain.Repositories;
using Core.MessageHandling;
using Core.Messages;

namespace Circles.Command.Application.Handlers
{
    public class RemoveUserCommandHandler : ICommandHandler<RemoveUsersCommand>
    {
        public UserCircleRepository _userCircleRepository { get; set; }
        public RemoveUserCommandHandler(UserCircleRepository userCircleRepository)
        {
            _userCircleRepository = userCircleRepository;
        }

        public async Task HandleAsync(RemoveUsersCommand command)
        {
            var deleteTask = command.Users.Select(
                uId => Task.Run(() => _userCircleRepository.DeleteByUserAndCircle(uId, command.CircleId))
            );
            await Task.WhenAll(deleteTask);
        }

        public async Task HandleAsync(BaseCommand command)
        {
            await HandleAsync((RemoveUsersCommand)command);
        }
    }
}