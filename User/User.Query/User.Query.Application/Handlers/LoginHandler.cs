using Core.DTOs;
using Core.MessageHandling;
using Microsoft.Extensions.DependencyInjection;
using User.Common.PasswordService;
using User.Query.Application.Queries;
using User.Query.Domain.Repositories;

namespace User.Query.Application.Handlers
{
    public class LoginHandler : IQueryHandler<LoginQuery, UserDto>
    {
        private readonly UserRepository _userRepository;
        private readonly PasswordHashService _hashService;

        public LoginHandler(IServiceProvider serviceProvider)
        {
            _userRepository = serviceProvider.GetRequiredService<UserRepository>();
            _hashService = serviceProvider.GetRequiredService<PasswordHashService>();
        }

        public async Task<UserDto> HandleAsync(LoginQuery query)
        {
            var user = await _userRepository.GetUserByUsernameAsync(query.Username);

            if (_hashService.VerifyPassword(query.Password, user.Password, user.Id))
            {
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
            else
            {
                throw new ArgumentException("Invalid username or password");
            }
        }
    }
}