using Core.MessageHandling;
using Microsoft.Extensions.DependencyInjection;
using User.Query.Application.DTOs;
using User.Query.Application.Queries;
using User.Query.Domain.Repositories;

namespace User.Query.Application.Handlers
{
    public class GetUserByUsernameQueryHandler : IQueryHandler<GetUserByUsernameQuery, UserDto>
    {
        private readonly UserRepository _userRepository;

        public GetUserByUsernameQueryHandler(IServiceProvider serviceProvider)
        {
            _userRepository = serviceProvider.GetRequiredService<UserRepository>();
        }

        public async Task<UserDto> HandleAsync(GetUserByUsernameQuery query)
        {
            return new UserDto( await _userRepository.GetUserByUsernameAsync(query.Username));
        }
    }
}