using Circles.Domain.Repositories;
using Circles.Query.Application.Queries;
using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;

namespace Circles.Query.Application.Handlers
{
    public class GetUsersByCircleIdQueryHandler : IMessageHandler<GetUsersByCircleIdQuery>
    {
        private readonly CirclesRepository _circlesRepository;

        public GetUsersByCircleIdQueryHandler(CirclesRepository circlesRepository)
        {
            _circlesRepository = circlesRepository;
        }

        public async Task<BaseResponse> HandleAsync(GetUsersByCircleIdQuery query)
        {
            CircleDto dto = new();
            dto = await _circlesRepository.GetUsersInCircle(query.CircleId);
            return new BaseResponse { ResponseCode = 200, Data = dto };
        }

        public async Task<BaseResponse> HandleAsync(BaseMessage query)
        {
            return await HandleAsync((GetUsersByCircleIdQuery)query);
        }
    }
}