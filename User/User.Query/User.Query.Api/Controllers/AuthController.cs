using Core.DTOs;
using Core.MessageHandling;
using Microsoft.AspNetCore.Mvc;
using User.Query.Application.DTOs;
using User.Query.Application.Exceptions;
using User.Query.Application.Queries;

namespace User.Query.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        
        private readonly IQueryDispatcher<ToknesDto> _authDispatcher;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IQueryDispatcher<ToknesDto> authDispatcher, ILogger<AuthController> logger)
        {
            _authDispatcher = authDispatcher;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> LoginUser([FromQuery] string username, string password) 
        {
            var loginQuery = new LoginQuery(username, password);
            ToknesDto tokens = new();
            try
            {
                tokens = await _authDispatcher.DispatchAsync(loginQuery);
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

            return StatusCode(200, tokens);
        }

        [HttpGet]
        [Route("token")]
        public async Task<IActionResult> RefreshToken([FromQuery] string refreshToken)
        {
            var refreshTokenQuery = new RefreshTokenQuery{ RefreshToken = refreshToken };
            ToknesDto tokens = new();
            try
            {
                tokens = await _authDispatcher.DispatchAsync(refreshTokenQuery);
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

            return StatusCode(200, tokens);
        }
    }
}