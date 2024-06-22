using Core.MessageHandling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using User.Query.Api.Config;
using User.Query.Application.DTOs;
using User.Query.Application.Exceptions;
using User.Query.Application.Queries;

namespace User.Query.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly IQueryDispatcher<LoginDto> _authDispatcher;
        private readonly ILogger<AuthController> _logger;
        private readonly IOptions<CookieConfig> _cookieConfig;

        public AuthController(IQueryDispatcher<LoginDto> authDispatcher, ILogger<AuthController> logger, IOptions<CookieConfig> cookieConfig)
        {
            _authDispatcher = authDispatcher;
            _logger = logger;
            _cookieConfig = cookieConfig;
        }

        [HttpPost]
        public async Task<IActionResult> LoginUser(LoginQuery loginQuery)
        {
            LoginDto loginDto = new();
            try
            {
                loginDto = await _authDispatcher.DispatchAsync(loginQuery);
            }
            catch (AuthException e)
            {
                return StatusCode(401, e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return StatusCode(400, "Something went wrong, please contact support using support page.");
            }

            var caccessOptions = new CookieOptions
            {
                HttpOnly = _cookieConfig.Value.HttpOnly,
                Secure = _cookieConfig.Value.Secure,
                SameSite = Enum.Parse<SameSiteMode>(_cookieConfig.Value.SameSite),
                MaxAge = TimeSpan.FromSeconds(_cookieConfig.Value.AccessMaxAge)
            };

            var refreshOptions = new CookieOptions
            {
                HttpOnly = _cookieConfig.Value.HttpOnly,
                Secure = _cookieConfig.Value.Secure,
                SameSite = Enum.Parse<SameSiteMode>(_cookieConfig.Value.SameSite),
                MaxAge = TimeSpan.FromHours(_cookieConfig.Value.RefreshMaxAge)
            };

            HttpContext.Response.Cookies.Append("AccessToken", loginDto.Tokens.AccessToken, caccessOptions);
            HttpContext.Response.Cookies.Append("RefreshToken", loginDto.Tokens.RefreshToken, refreshOptions);

            return StatusCode(200, loginDto.User);
        }

        [HttpPost]
        [Route("token")]
        public async Task<IActionResult> RefreshToken()
        {
            if (Request.Cookies.TryGetValue("RefreshToken", out string refreshToken))
            {
                var refreshTokenQuery = new RefreshTokenQuery
                {
                    RefreshToken = refreshToken

                };

                TokensDto tokens = new();

                try
                {
                    var loginDto = await _authDispatcher.DispatchAsync(refreshTokenQuery);
                    tokens = loginDto.Tokens;
                }
                catch (AuthException e)
                {
                    return StatusCode(401, e.Message);
                }
                catch (Exception e)
                {
                    _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                    return StatusCode(400, "Something went wrong, please contact support using support page.");
                }

                var caccessOptions = new CookieOptions
                {
                    HttpOnly = _cookieConfig.Value.HttpOnly,
                    Secure = _cookieConfig.Value.Secure,
                    SameSite = Enum.Parse<SameSiteMode>(_cookieConfig.Value.SameSite),
                    MaxAge = TimeSpan.FromSeconds(_cookieConfig.Value.AccessMaxAge)
                };

                var refreshOptions = new CookieOptions
                {
                    HttpOnly = _cookieConfig.Value.HttpOnly,
                    Secure = _cookieConfig.Value.Secure,
                    SameSite = Enum.Parse<SameSiteMode>(_cookieConfig.Value.SameSite),
                    MaxAge = TimeSpan.FromHours(_cookieConfig.Value.RefreshMaxAge)
                };

                HttpContext.Response.Cookies.Append("AccessToken", tokens.AccessToken, caccessOptions);
                HttpContext.Response.Cookies.Append("RefreshToken", tokens.RefreshToken, refreshOptions);

                return StatusCode(200);
            }
            else
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = _cookieConfig.Value.HttpOnly,
                    Secure = _cookieConfig.Value.Secure,
                    SameSite = Enum.Parse<SameSiteMode>(_cookieConfig.Value.SameSite),
                    Expires = DateTime.Now.AddDays(-1)
                };

                HttpContext.Response.Cookies.Append("AccessToken", "", cookieOptions);
                HttpContext.Response.Cookies.Append("RefreshToken", "", cookieOptions);
                
                return StatusCode(400, "Unauthorized");
            }
        }

        [HttpPost]
        [Route("logout")]
        public IActionResult Logout()
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = _cookieConfig.Value.HttpOnly,
                Secure = _cookieConfig.Value.Secure,
                SameSite = Enum.Parse<SameSiteMode>(_cookieConfig.Value.SameSite),
                Expires = DateTime.Now.AddDays(-1)
            };

            HttpContext.Response.Cookies.Append("AccessToken", "", cookieOptions);
            HttpContext.Response.Cookies.Append("RefreshToken", "", cookieOptions);

            return StatusCode(200);
        }
    }
}