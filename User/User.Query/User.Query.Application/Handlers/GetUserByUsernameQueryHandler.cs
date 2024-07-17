using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;
using User.Query.Application.Queries;
using User.Query.Domain.Repositories;

namespace User.Query.Application.Handlers
{
    public class GetUserByUsernameQueryHandler : IMessageHandler<GetUserByUsernameQuery>
    {
        private readonly UserRepository _userRepository;
        private readonly ILogger<GetUserByUsernameQueryHandler> _logger;

        public GetUserByUsernameQueryHandler(UserRepository userRepository, ILogger<GetUserByUsernameQueryHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<BaseResponse> HandleAsync(GetUserByUsernameQuery query)
        {
            try
            {
                var user = await _userRepository.GetUserByUsernameAsync(query.Username);
                return new BaseResponse { ResponseCode = 200, Data = user };
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return new BaseResponse { ResponseCode = 404, Message = "User not found." };
            }
            
        }

        public async Task<BaseResponse> HandleAsync(BaseMessage query)
        {
            return await HandleAsync((GetUserByUsernameQuery)query);
        }
    }
}