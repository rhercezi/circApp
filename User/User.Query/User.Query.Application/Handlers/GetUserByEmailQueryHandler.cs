using Core.MessageHandling;
using Microsoft.Extensions.DependencyInjection;
using User.Query.Application.DTOs;
using User.Query.Application.Queries;
using User.Query.Domain.Repositories;

namespace User.Query.Application.Handlers
{
    public class GetUserByEmailQueryHandler : IQueryHandler<GetUserByEmailQuery, UserDto>
    {
        private readonly UserRepository _userRepository;

        public GetUserByEmailQueryHandler(IServiceProvider serviceProvider)
        {
            _userRepository = serviceProvider.GetRequiredService<UserRepository>();
        }

        public async Task<UserDto> HandleAsync(GetUserByEmailQuery query)
        {
            return new UserDto( await _userRepository.GetUserByEmailAsync(query.Email));
        }
    }
}