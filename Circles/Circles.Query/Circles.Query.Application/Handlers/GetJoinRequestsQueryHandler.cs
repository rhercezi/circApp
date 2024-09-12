using Circles.Domain.Repositories;
using Circles.Query.Application.DTOs;
using Circles.Query.Application.Queries;
using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;

namespace Circles.Query.Application.Handlers
{
    public class GetJoinRequestsQueryHandler : IMessageHandler<GetJoinRequestsQuery>
    {
        private readonly UserRepository _userRepository;
        private readonly JoinRequestRepository _joinRequestRepository;
        private readonly CirclesRepository _circleRepository;

        public GetJoinRequestsQueryHandler(UserRepository userRepository,
                                           JoinRequestRepository joinRequestRepository,
                                           CirclesRepository circleRepository)
        {
            _userRepository = userRepository;
            _joinRequestRepository = joinRequestRepository;
            _circleRepository = circleRepository;
        }

        public async Task<BaseResponse> HandleAsync(GetJoinRequestsQuery message)
        {
            var joinRequests = await _joinRequestRepository.FindAsync(jr => jr.UserId == message.UserId);
            if (joinRequests.Count == 0) return new BaseResponse { ResponseCode = 204, Message = "No join requests found." };
            var circles = await _circleRepository.GetByIdsAsync(joinRequests.Select(jr => jr.CircleId).ToList());
            var users = await _userRepository.GetByIdsAsync(joinRequests.Select(jr => jr.InviterId).ToList());
            var dto = joinRequests.Select(jr => {
                var circle = circles.FirstOrDefault(c => c.CircleId == jr.CircleId);
                var user = users.FirstOrDefault(u => u.UserId == jr.InviterId);
                return new JoinRequestDto
                {
                    RequestId = jr.Id,
                    CircleId = jr.CircleId,
                    CircleName = circle.Name,
                    InviterName = user.FirstName,
                    InviterSurname = user.FamilyName,
                    InviterUserName = user.UserName
                };
            }).ToList();

            return new BaseResponse { ResponseCode = 200, Data = dto };
        }

        public Task<BaseResponse> HandleAsync(BaseMessage message)
        {
            return HandleAsync((GetJoinRequestsQuery)message);
        }
    }
}