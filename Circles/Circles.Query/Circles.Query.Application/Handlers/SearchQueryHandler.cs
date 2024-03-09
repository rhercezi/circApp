using Circles.Domain.Repositories;
using Circles.Query.Application.Queries;
using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;

namespace Circles.Query.Application.Handlers
{
    public class SearchQueryHandler : IQueryHandler<SearchQuery, AppUsersDto>
    {
        public readonly UserRepository _userRepository;
        public SearchQueryHandler(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<AppUsersDto> HandleAsync(SearchQuery query)
        {
            var dto = new AppUsersDto();
            dto = await _userRepository.SearchUsersAsync(query.QWord);
            return dto;
        }

        public async Task<BaseDto> HandleAsync(BaseQuery query)
        {
            return await HandleAsync((SearchQuery)query);
        }
    }
}