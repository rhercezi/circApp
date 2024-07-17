using Circles.Command.Application.Commands;
using Circles.Domain.Repositories;
using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;

namespace Circles.Command.Application.Handlers
{
    public class DeleteCircleCommandHabndler : IMessageHandler<DeleteCircleCommand>
    {
        private readonly CirclesRepository _circlesRepository;
        private readonly UserCircleRepository _userCircleRepository;
        private readonly ILogger<DeleteCircleCommandHabndler> _logger;
        public DeleteCircleCommandHabndler(CirclesRepository circlesRepository,
                                           UserCircleRepository userCircleRepository,
                                           ILogger<DeleteCircleCommandHabndler> logger)
        {
            _circlesRepository = circlesRepository;
            _userCircleRepository = userCircleRepository;
            _logger = logger;
        }

        public async Task<BaseResponse> HandleAsync(DeleteCircleCommand command)
        {
            using var session = await _circlesRepository.GetSession();

            try
            {
                session.StartTransaction();
                await _userCircleRepository.DeleteByCircle(command.CircleId);
                await _circlesRepository.DeleteCircle(command.CircleId);
                session.CommitTransaction();

                return new BaseResponse { ResponseCode = 204 };
            }
            catch (Exception e)
            {
                session.AbortTransaction();
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return new BaseResponse { ResponseCode = 500, Message = "Something went wrong, please try again later." };
            }
        }

        public async Task<BaseResponse> HandleAsync(BaseMessage command)
        {
            return await HandleAsync((DeleteCircleCommand)command);
        }
    }
}