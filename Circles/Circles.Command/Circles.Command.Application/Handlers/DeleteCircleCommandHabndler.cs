using Circles.Command.Application.Commands;
using Circles.Domain.Repositories;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;

namespace Circles.Command.Application.Handlers
{
    public class DeleteCircleCommandHabndler : ICommandHandler<DeleteCircleCommand>
    {
        public readonly CirclesRepository _circlesRepository;
        public readonly UserCircleRepository _userCircleRepository;
        public readonly ILogger<DeleteCircleCommandHabndler> _logger;
        public DeleteCircleCommandHabndler(CirclesRepository circlesRepository,
                                           UserCircleRepository userCircleRepository,
                                           ILogger<DeleteCircleCommandHabndler> logger)
        {
            _circlesRepository = circlesRepository;
            _userCircleRepository = userCircleRepository;
            _logger = logger;
        }

        public async Task HandleAsync(DeleteCircleCommand command)
        {
            using var session = await _circlesRepository.GetSession();

            try
            {
                session.StartTransaction();
                await _userCircleRepository.DeleteByCircle(command.CircleId);
                await _circlesRepository.DeleteCircle(command.CircleId);
                session.CommitTransaction();
            }
            catch (Exception e)
            {
                session.AbortTransaction();
                _logger.LogError($"{e.Message}\n{e.StackTrace}");
                throw;
            }
        }

        public async Task HandleAsync(BaseCommand command)
        {
            await HandleAsync((DeleteCircleCommand)command);
        }
    }
}