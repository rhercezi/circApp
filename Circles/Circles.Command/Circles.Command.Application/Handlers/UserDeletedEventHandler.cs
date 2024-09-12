using Circles.Domain.Repositories;
using Core.DTOs;
using Core.Events.PublicEvents;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;

namespace Circles.Command.Application.Handlers
{
    public class UserDeletedEventHandler : IMessageHandler<UserDeletedPublicEvent>
    {
        private readonly UserRepository _userRepository;
        private readonly UserCircleRepository _userCircleRepository;
        private readonly JoinRequestRepository _joinRequestRepository;
        private readonly CirclesRepository _circlesRepository;
        private readonly ILogger<UserDeletedEventHandler> _logger;

        public UserDeletedEventHandler(UserRepository userRepository,
                                       UserCircleRepository userCircleRepository,
                                       JoinRequestRepository joinRequestRepository,
                                       ILogger<UserDeletedEventHandler> logger,
                                       CirclesRepository circlesRepository)
        {
            _userRepository = userRepository;
            _userCircleRepository = userCircleRepository;
            _joinRequestRepository = joinRequestRepository;
            _logger = logger;
            _circlesRepository = circlesRepository;
        }

        public async Task<BaseResponse> HandleAsync(UserDeletedPublicEvent xEvent)
        {
            using var session = await _userRepository.GetSession();
            try
            {
                session.StartTransaction();
                await _userRepository.DeleteUser(xEvent.Id);
                ChangeCircleOwner(xEvent);
                await _userCircleRepository.DeleteByUser(xEvent.Id);
                await _joinRequestRepository.DeleteAsync(jr => jr.UserId == xEvent.Id  || jr.InviterId == xEvent.Id);
                session.CommitTransaction();

                return new BaseResponse { ResponseCode = 200 };
            }
            catch (Exception e)
            {
                session.AbortTransaction();
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return new BaseResponse { ResponseCode = 500 };
            }
        }

        private async void ChangeCircleOwner(UserDeletedPublicEvent xEvent)
        {
            var user = await _userRepository.GetCirclesForUser(xEvent.Id);
            if (user.Circles == null || user.Circles.Count == 0) return;

            foreach (var circle in user.Circles)
            {
                var users = await _userCircleRepository.FindAsync(uc => uc.CircleId == circle.Id && uc.UserId != xEvent.Id);
                if (users.Count > 0)
                {
                    await _circlesRepository.ChangeCircleOwner(circle.Id, users[0].UserId);
                }
                else
                {
                    await _circlesRepository.DeleteCircle(circle.Id);
                }
            }
        }

        public async Task<BaseResponse> HandleAsync(BaseMessage xEvent)
        {
            return await HandleAsync((UserDeletedPublicEvent)xEvent);
        }
    }
}