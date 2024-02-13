using Core.DTOs;
using Core.MessageHandling;
using Microsoft.Extensions.DependencyInjection;
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
            var user = await _userRepository.GetUserByEmailAsync(query.Email);
            return new UserDto
            {
                Id = user.Id,
                Created = user.Created,
                Updated = user.Updated,
                UserName = user.UserName,
                FirstName = user.FirstName,
                FamilyName = user.FamilyName,
                Email = user.Email,
                EmailVerified = user.EmailVerified
            };
        }
    }
}