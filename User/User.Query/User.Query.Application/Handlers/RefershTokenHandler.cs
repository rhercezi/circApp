using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Core.Utilities;
using Microsoft.Extensions.Logging;
using User.Query.Application.DTOs;
using User.Query.Application.Exceptions;
using User.Query.Application.Queries;
using User.Query.Domain.Entities;
using User.Query.Domain.Repositories;

namespace User.Query.Application.Handlers
{
    public class RefershTokenHandler : IQueryHandler<RefreshTokenQuery, ToknesDto>
    {
        private readonly JwtService _jwtService;
        private readonly RefreshTokenRepository _refreshTokenRepository;
        private readonly UserRepository _userRepository;
        private readonly ILogger<RefershTokenHandler> _logger;

        public RefershTokenHandler(JwtService jwtService,
                                   RefreshTokenRepository refreshTokenRepository,
                                   UserRepository userRepository,
                                   ILogger<RefershTokenHandler> logger)
        {
            _jwtService = jwtService;
            _refreshTokenRepository = refreshTokenRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ToknesDto> HandleAsync(RefreshTokenQuery query)
        {
            if (!string.IsNullOrWhiteSpace(query.RefreshToken) && _jwtService.ValidateToken(query.RefreshToken))
            {
                var clames = _jwtService.GetTokenClaims(query.RefreshToken);
                var tokenId = clames.FirstOrDefault(x => x.Type == "jti")?.Value;
                if (!string.IsNullOrWhiteSpace(tokenId))
                {
                    var tokenModel = await _refreshTokenRepository.GetRefreshTokenById(tokenId);
                    if (tokenModel != null)
                    {
                        if (tokenModel.UserId == clames.FirstOrDefault(x => x.Type == "sub")?.Value &&
                            tokenModel.Iat == long.Parse(clames.FirstOrDefault(x => x.Type == "iat")?.Value) &&
                            tokenModel.Nbf == long.Parse(clames.FirstOrDefault(x => x.Type == "nbf")?.Value) &&
                            tokenModel.Exp == long.Parse(clames.FirstOrDefault(x => x.Type == "exp")?.Value))
                        {
                            try
                            {
                                var user = await _userRepository.GetUserByIdAsync(Guid.Parse(tokenModel.UserId));
                                var newTokens = new ToknesDto
                                {
                                    AccessToken = _jwtService.GenerateAccessToken(user.Id.ToString(), user.FirstName, user.FamilyName),
                                    RefreshToken = _jwtService.GenerateRefreshToken(user.Id.ToString(), user.FirstName, user.FamilyName)
                                };
    
                                var refreshTokenModel = new RefreshTokenEntity
                                {
                                    Id = _jwtService.GetTokenClaims(newTokens.RefreshToken).FirstOrDefault(x => x.Type == "jti")?.Value,
                                    UserId = user.Id.ToString(),
                                    Iat = long.Parse(_jwtService.GetTokenClaims(newTokens.RefreshToken).FirstOrDefault(x => x.Type == "iat")?.Value),
                                    Nbf = long.Parse(_jwtService.GetTokenClaims(newTokens.RefreshToken).FirstOrDefault(x => x.Type == "nbf")?.Value),
                                    Exp = long.Parse(_jwtService.GetTokenClaims(newTokens.RefreshToken).FirstOrDefault(x => x.Type == "exp")?.Value)
                                };
    
                                await _refreshTokenRepository.DeleteRefreshToken(tokenModel);
                                await _refreshTokenRepository.AddRefreshToken(refreshTokenModel);
    
                                return newTokens;
                            }
                            catch (Exception e)
                            {
                                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                                throw;
                            }
                        }
                    }
                }
            }
            throw new AuthException("Invalid refresh token.");
        }

        public async Task<BaseDto> HandleAsync(BaseQuery query)
        {
            return await HandleAsync((RefreshTokenQuery)query);
        }
    }
}