using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using User.Query.Application.Utils.Configs;
using User.Query.Domain.Entities;

namespace User.Query.Application.Utils.Services
{
    public class JwtService
    {
        private readonly JwtConfig _jwtConfig;
        private readonly ILogger<JwtService> _logger;
        public JwtService(IOptions<JwtConfig> config, ILogger<JwtService> logger)
        {
            _jwtConfig = config.Value;
            _logger = logger;
        }

        public string GetToken(UserEntity user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secret = Encoding.ASCII.GetBytes(_jwtConfig.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("id", user.Id.ToString()),
                    new Claim("firstName", user.FirstName),
                    new Claim("lastName", user.FamilyName)
                }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha512Signature)
            };

            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }

        public List<Claim> ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secret = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters{
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secret),
                    ValidateIssuer = false,
                    ValidateAudience = false, 
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validToken);

                var tokenJ = (JwtSecurityToken)validToken;
                return tokenJ.Claims.ToList();
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message, e.StackTrace);
                return new List<Claim>();
            }
        }
    }
}