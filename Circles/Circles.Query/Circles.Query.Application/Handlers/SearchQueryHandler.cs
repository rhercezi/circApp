using Circles.Domain.Repositories;
using Circles.Query.Application.Queries;
using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;

namespace Circles.Query.Application.Handlers
{
    public class SearchQueryHandler : IMessageHandler<SearchQuery>
    {
        public readonly UserRepository _userRepository;
        public SearchQueryHandler(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<BaseResponse> HandleAsync(SearchQuery query)
        {
            if (string.IsNullOrEmpty(query.QWord))
            {
                return new BaseResponse { ResponseCode = 200, Message = {} };
            }
            var dto = new AppUsersDto();
            dto = await _userRepository.SearchUsersAsync(query.QWord);
            return new BaseResponse { ResponseCode = 200, Data = dto };
        }

        public async Task<BaseResponse> HandleAsync(BaseMessage query)
        {
            return await HandleAsync((SearchQuery)query);
        }
    }
}