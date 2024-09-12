using Circles.Command.Application.Commands;
using Circles.Domain.Entities;
using Circles.Domain.Repositories;
using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;

namespace Circles.Command.Application.Handlers
{
    public class ConfirmJoinCommandHandler : IMessageHandler<ConfirmJoinCommand>
    {
        private readonly UserCircleRepository _userCircleRepository;
        private readonly JoinRequestRepository _requestRepository;
        private readonly ILogger<ConfirmJoinCommandHandler> _logger;
        public ConfirmJoinCommandHandler(UserCircleRepository userCircleRepository,
                                         JoinRequestRepository requestRepository,
                                         ILogger<ConfirmJoinCommandHandler> logger)
        {
            _userCircleRepository = userCircleRepository;
            _requestRepository = requestRepository;
            _logger = logger;
        }

        public async Task<BaseResponse> HandleAsync(ConfirmJoinCommand command)
        {
            if (!RequestExists(command)) return new BaseResponse { ResponseCode = 404, Message = $"Request for join confirmation does not exist {command}" };

            using var session = await _userCircleRepository.GetSession();
            try
            {
                session.StartTransaction();

                if (command.IsAccepted)
                {
                    await _userCircleRepository.SaveAsync(
                    new UserCircleModel
                    {
                        UserId = command.UserId,
                        CircleId = command.CircleId
                    }
                );
                }
    
                await _requestRepository.DeleteAsync(jr => jr.CircleId == command.CircleId && jr.UserId == command.UserId);

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

        private bool RequestExists(ConfirmJoinCommand command)
        {
            var result = _requestRepository.FindAsync(jr => jr.CircleId == command.CircleId && jr.UserId == command.UserId).Result;
            if (result.Count > 0) return true;
            return false;
        }

        public async Task<BaseResponse> HandleAsync(BaseMessage command)
        {
            return await HandleAsync((ConfirmJoinCommand)command);
        }
    }
}