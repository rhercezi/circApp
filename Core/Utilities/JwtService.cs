using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Core.Configs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Core.Utilities
{
    public class JwtService
    {
        private readonly JwtConfig _config;
        private readonly ILogger<JwtService> _logger;

        public JwtService(IOptions<JwtConfig> config, ILogger<JwtService> logger)
        {
            _config = config.Value;
            _logger = logger;
        }

        public string GenerateAccessToken(Guid id, string firstName, string familyName)
        {
            return GenerateToken(id, firstName, familyName, TimeSpan.FromMinutes(5));
        }

        public string GenerateRefreshToken(Guid id, string firstName, string familyName)
        {
            return GenerateToken(id, firstName, familyName, TimeSpan.FromDays(5));
        }

        private string GenerateToken(Guid id, string firstName, string familyName, TimeSpan expiration)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, id.ToString()),
                new Claim(ClaimTypes.GivenName, firstName),
                new Claim(ClaimTypes.Name, familyName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(), ClaimValueTypes.Integer64)
            };

            var key = new SymmetricSecurityKey(Convert.FromBase64String(_config.SigningKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                _config.Issuer,
                _config.Audience,
                claims,
                expires: DateTime.UtcNow.Add(expiration),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _config.Issuer,
                ValidateAudience = true,
                ValidAudience = _config.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(_config.SigningKey)),
                ValidateLifetime = true
            };

            try
            {
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch (Exception e)
            {
                _logger.LogError(e.StackTrace, e.Message);
                return null;
            }
        }
    }
}