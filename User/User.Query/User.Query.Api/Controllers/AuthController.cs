using Core.DTOs;
using Core.MessageHandling;
using Microsoft.AspNetCore.Mvc;
using User.Query.Application.Queries;

namespace User.Query.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        
        private readonly IQueryDispatcher<UserDto> _authDispatcher;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IQueryDispatcher<UserDto> authDispatcher, ILogger<AuthController> logger)
        {
            _authDispatcher = authDispatcher;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> LoginUser([FromQuery] string username, string password) 
        {
            var loginQuery = new LoginQuery(username, password);
            UserDto user;
            try
            {
                user = await _authDispatcher.DispatchAsync(loginQuery);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return StatusCode(400, "Something went wrong, please contact support using support page.");
            }

            if (!user.EmailVerified)
            {
                return StatusCode(401, "Email is not verified, please verify your email by clicking on the link sent to your email address");
            }

            return StatusCode(200, user);
        }
    }
}