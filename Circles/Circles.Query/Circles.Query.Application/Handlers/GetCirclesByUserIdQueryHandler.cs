using Circles.Domain.Entities;
using Circles.Domain.Repositories;
using Circles.Query.Application.Queries;
using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.IdentityModel.Tokens;

namespace Circles.Query.Application.Handlers
{
    public class GetCirclesByUserIdQueryHandler : IMessageHandler<GetCirclesByUserIdQuery>
    {
        private readonly UserRepository _userRepository;

        public GetCirclesByUserIdQueryHandler(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<BaseResponse> HandleAsync(GetCirclesByUserIdQuery query)
        {
            AppUserDto dto = new();
            dto = await _userRepository.GetCirclesForUser(query.UserId);
            return new BaseResponse { ResponseCode = 200, Data = dto };
        }

        public async Task<BaseResponse> HandleAsync(BaseMessage query)
        {
            return await HandleAsync((GetCirclesByUserIdQuery)query);
        }
    }
}