using Core.MessageHandling;
using Microsoft.Extensions.DependencyInjection;
using User.Common.PasswordService;
using User.Query.Application.DTOs;
using User.Query.Application.Queries;
using User.Query.Domain.Repositories;

namespace User.Query.Application.Handlers
{
    public class LoginHandler : IQueryHandler<LoginQuery, TokenDto>
    {
        private readonly UserRepository _userRepository;
        private readonly PasswordHashService _hashService; 

        public LoginHandler(IServiceProvider serviceProvider)
        {
            _userRepository = serviceProvider.GetRequiredService<UserRepository>();
            _hashService = serviceProvider.GetRequiredService<PasswordHashService>();
        }

        public async Task<TokenDto> HandleAsync(LoginQuery query)
        {
            var user = await _userRepository.GetUserByUsernameAsync(query.Username);

            if (_hashService.VerifyPassword(query.Password, user.Password, user.Id))
            {
                return new TokenDto{ Token = "jksdhfjk" };
            }
            return new TokenDto { Token = "fdjlskjlk" };
        }
    }
}