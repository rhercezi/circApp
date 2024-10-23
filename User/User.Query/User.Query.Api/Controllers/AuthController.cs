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

        private readonly IMessageDispatcher _authDispatcher;
        private readonly ILogger<AuthController> _logger;
        private readonly IOptions<CookieConfig> _cookieConfig;

        public AuthController(IMessageDispatcher authDispatcher, ILogger<AuthController> logger, IOptions<CookieConfig> cookieConfig)
        {
            _authDispatcher = authDispatcher;
            _logger = logger;
            _cookieConfig = cookieConfig;
        }

        [HttpPost]
        public async Task<IActionResult> LoginUser(LoginQuery loginQuery)
        {
            try
            {
                var response = await _authDispatcher.DispatchAsync(loginQuery);
                if (response.ResponseCode < 300 && response.Data != null)
                {
                    var loginDto = (LoginDto)response.Data;

                    var accessOptions = new CookieOptions
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

                    HttpContext.Response.Cookies.Append("AccessToken", loginDto.Tokens.AccessToken, accessOptions);
                    HttpContext.Response.Cookies.Append("RefreshToken", loginDto.Tokens.RefreshToken, refreshOptions);

                    loginDto.Tokens = null;
                    return StatusCode(200, loginDto);
                }
                else
                {
                    return StatusCode(response.ResponseCode, response.Message);
                }
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

                RefreshDto refreshDto = new();

                try
                {
                    var response = await _authDispatcher.DispatchAsync(refreshTokenQuery);
                    if (response.ResponseCode < 300 && response.Data != null)
                    {
                        refreshDto = (RefreshDto)response.Data;
                    }
                    else
                    {
                        return StatusCode(response.ResponseCode, response.Message);
                    }
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

                var accessOptions = new CookieOptions
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

                HttpContext.Response.Cookies.Append("AccessToken", refreshDto.Tokens.AccessToken!, accessOptions);
                HttpContext.Response.Cookies.Append("RefreshToken", refreshDto.Tokens.RefreshToken!, refreshOptions);

                refreshDto.Tokens = null;
                return StatusCode(200, refreshDto);
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