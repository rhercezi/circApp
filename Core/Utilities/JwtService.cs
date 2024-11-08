using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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

        public string GenerateAccessToken(string id, string firstName, string familyName)
        {
            return GenerateToken(id, firstName, familyName, TimeSpan.FromMinutes(_config.TokenExpiration));
        }

        public string GenerateRefreshToken(string id, string firstName, string familyName)
        {
            return GenerateToken(id, firstName, familyName, TimeSpan.FromDays(_config.RefreshTokenExpiration));
        }

        private string GenerateToken(string id, string firstName, string familyName, TimeSpan expiration)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, id.ToString()),
                new Claim(ClaimTypes.GivenName, firstName),
                new Claim(ClaimTypes.Name, familyName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(), ClaimValueTypes.Integer64)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.SigningKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _config.Issuer,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(expiration),
                SigningCredentials = credentials
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);

            return  handler.WriteToken(token);
        }

        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _config.Issuer,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.SigningKey))
            };

            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out _);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static IEnumerable<Claim> GetTokenClaims(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.Claims;
        }

        public static string GetClaimValue(string token, string claimType)
        {
            var claims = GetTokenClaims(token);
            return claims.FirstOrDefault(c => c.Type == claimType)?.Value ?? string.Empty;
        }
    }
}