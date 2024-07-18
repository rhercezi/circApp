using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Core.Utilities;
using Microsoft.Extensions.Logging;
using User.Query.Application.DTOs;
using User.Query.Application.Queries;
using User.Query.Domain.Entities;
using User.Query.Domain.Repositories;

namespace User.Query.Application.Handlers
{
    public class RefershTokenHandler : IMessageHandler<RefreshTokenQuery>
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

        public async Task<BaseResponse> HandleAsync(RefreshTokenQuery query)
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
                                var newTokens = new TokensDto
                                {
                                    AccessToken = _jwtService.GenerateAccessToken(user.Id.ToString(), user.FirstName, user.FamilyName),
                                    RefreshToken = _jwtService.GenerateRefreshToken(user.Id.ToString(), user.FirstName, user.FamilyName)
                                };

                                var loginDto = new LoginDto
                                {
                                    Tokens = newTokens,
                                    User = new UserDto
                                    {
                                        Id = user.Id,
                                        Created = user.Created,
                                        Updated = user.Updated,
                                        UserName = user.UserName,
                                        FirstName = user.FirstName,
                                        FamilyName = user.FamilyName,
                                        Email = user.Email,
                                        EmailVerified = user.EmailVerified
                                    }
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

                                return new BaseResponse { ResponseCode = 200, Data = loginDto };
                            }
                            catch (Exception e)
                            {
                                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                                return new BaseResponse { ResponseCode = 500, Message = "Something went wrong, please contact support using support page." };
                            }
                        }
                    }
                }
            }
            return new BaseResponse { ResponseCode = 401, Message = "Invalid token." };
        }

        public async Task<BaseResponse> HandleAsync(BaseMessage query)
        {
            return await HandleAsync((RefreshTokenQuery)query);
        }
    }
}