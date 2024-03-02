using Circles.Command.Application.Commands;
using Circles.Domain.Entities;
using Circles.Domain.Repositories;
using Core.MessageHandling;
using Core.Messages;

namespace Circles.Command.Application.Handlers
{
    public class ConfirmJoinCommandHandler : ICommandHandler<ConfirmJoinCommand>
    {
        public UserCircleRepository _userCircleRepository { get; set; }
        public JoinRequestRepository _requestRepository { get; set; }
        public ConfirmJoinCommandHandler(UserCircleRepository userCircleRepository, JoinRequestRepository requestRepository)
        {
            _userCircleRepository = userCircleRepository;
            _requestRepository = requestRepository;
        }

        public async Task HandleAsync(ConfirmJoinCommand command)
        {
            await _userCircleRepository.SaveAsync(
                new UserCircleModel
                {
                    UserId = command.UserId,
                    CircleId = command.CircleId
                }
            );

            await _requestRepository.DeleteByPredicate(r => r.UserId == command.Id && r.CircleId == command.CircleId);
        }

        public async Task HandleAsync(BaseCommand command)
        {
            await HandleAsync((ConfirmJoinCommand)command);
        }
    }
}