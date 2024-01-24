using Core.MessageHandling;
using Microsoft.Extensions.DependencyInjection;
using User.Query.Application.DTOs;
using User.Query.Application.Queries;
using User.Query.Domain.Repositories;

namespace User.Query.Application.Handlers
{
    public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserDto>
    {
        private readonly UserRepository _userRepository;

        public GetUserByIdQueryHandler(IServiceProvider serviceProvider)
        {
            _userRepository = serviceProvider.GetRequiredService<UserRepository>();
        }

        public async Task<UserDto> HandleAsync(GetUserByIdQuery query)
        {
            return new UserDto( await _userRepository.GetUserByIdAsync(query.Id));
        }
    }
}