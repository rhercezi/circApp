using Circles.Command.Application.Commands;
using Circles.Domain.Entities;
using Circles.Domain.Repositories;
using Core.MessageHandling;
using Core.Messages;

namespace Circles.Command.Application.Handlers
{
    public class ConfirmJoinCommandHandler : ICommandHandler<ConfirmJoinCommand>
    {
        public readonly UserCircleRepository _userCircleRepository;
        public readonly JoinRequestRepository _requestRepository;
        public ConfirmJoinCommandHandler(UserCircleRepository userCircleRepository, JoinRequestRepository requestRepository)
        {
            _userCircleRepository = userCircleRepository;
            _requestRepository = requestRepository;
        }

        public async Task HandleAsync(ConfirmJoinCommand command)
        {
            if (!RequestExists(command)) throw new CirclesValidationException($"Request for join confirmation does not exist {command}");

            await _userCircleRepository.SaveAsync(
                new UserCircleModel
                {
                    UserId = command.UserId,
                    CircleId = command.CircleId
                }
            );

            await _requestRepository.DeleteAsync(command.UserId, command.CircleId);
        }

        private bool RequestExists(ConfirmJoinCommand command)
        {
            var result = _requestRepository.FindAsync(jr => jr.CircleId == command.CircleId && jr.UserId == command.UserId).Result;
            if (result.Count > 0) return true;
            return false;
        }

        public async Task HandleAsync(BaseCommand command)
        {
            await HandleAsync((ConfirmJoinCommand)command);
        }
    }
}