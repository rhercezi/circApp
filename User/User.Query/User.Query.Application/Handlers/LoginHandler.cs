using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Core.Utilities;
using Microsoft.Extensions.Logging;
using User.Common.PasswordService;
using User.Query.Application.DTOs;
using User.Query.Application.Exceptions;
using User.Query.Application.Queries;
using User.Query.Domain.Entities;
using User.Query.Domain.Repositories;

namespace User.Query.Application.Handlers
{
    public class LoginHandler : IQueryHandler<LoginQuery, ToknesDto>
    {
        private readonly UserRepository _userRepository;
        private readonly PasswordHashService _hashService;
        private readonly JwtService _jwtService;
        private readonly RefreshTokenRepository _refreshTokenRepository;
        private readonly ILogger<LoginHandler> _logger;

        public LoginHandler(UserRepository userRepository,
                            PasswordHashService hashService,
                            JwtService jwtService,
                            RefreshTokenRepository refreshTokenRepository,
                            ILogger<LoginHandler> logger)
        {
            _userRepository = userRepository;
            _hashService = hashService;
            _jwtService = jwtService;
            _refreshTokenRepository = refreshTokenRepository;
            _logger = logger;
        }

        public async Task<ToknesDto> HandleAsync(LoginQuery query)
        {
            var user = await _userRepository.GetUserByUsernameAsync(query.Username);

            if (_hashService.VerifyPassword(query.Password, user.Password, user.Id))
            {
                if (user.EmailVerified == false)
                {
                    throw new AuthException("Email is not verified, please verify your email by clicking on the link sent to your email address");
                }

                ToknesDto tokens = new();
                tokens.AccessToken = _jwtService.GenerateAccessToken(user.Id.ToString(), user.FirstName, user.FamilyName);
                tokens.RefreshToken = _jwtService.GenerateRefreshToken(user.Id.ToString(), user.FirstName, user.FamilyName);

                try
                {
                    var refreshTokenModel = new RefreshTokenEntity
                    {
                        Id = _jwtService.GetTokenClaims(tokens.RefreshToken).FirstOrDefault(x => x.Type == "jti")?.Value,
                        UserId = user.Id.ToString(),
                        Iat = long.Parse(_jwtService.GetTokenClaims(tokens.RefreshToken).FirstOrDefault(x => x.Type == "iat")?.Value),
                        Nbf = long.Parse(_jwtService.GetTokenClaims(tokens.RefreshToken).FirstOrDefault(x => x.Type == "nbf")?.Value),
                        Exp = long.Parse(_jwtService.GetTokenClaims(tokens.RefreshToken).FirstOrDefault(x => x.Type == "exp")?.Value)
                    };
                    await _refreshTokenRepository.AddRefreshToken(refreshTokenModel);
                }
                catch (Exception e)
                {
                    _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                    throw;
                }

                return tokens;
            }
            else
            {
                throw new AuthException("Invalid username or password");
            }
        }

        public async Task<BaseDto> HandleAsync(BaseQuery query)
        {
            return await HandleAsync((LoginQuery)query);
        }
    }
}