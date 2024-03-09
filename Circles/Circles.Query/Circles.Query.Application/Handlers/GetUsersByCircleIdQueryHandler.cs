using Circles.Domain.Repositories;
using Circles.Query.Application.Queries;
using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;

namespace Circles.Query.Application.Handlers
{
    public class GetUsersByCircleIdQueryHandler : IQueryHandler<GetUsersByCircleIdQuery, CircleDto>
    {
        private readonly CirclesRepository _circlesRepository;

        public GetUsersByCircleIdQueryHandler(CirclesRepository circlesRepository)
        {
            _circlesRepository = circlesRepository;
        }

        public async Task<CircleDto> HandleAsync(GetUsersByCircleIdQuery query)
        {
            CircleDto dto = new();
            dto = await _circlesRepository.GetUsersInCircle(query.CircleId);
            return dto;
        }

        public async Task<BaseDto> HandleAsync(BaseQuery query)
        {
            return await HandleAsync((GetUsersByCircleIdQuery)query);
        }
    }
}