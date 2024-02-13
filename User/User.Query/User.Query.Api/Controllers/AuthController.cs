using Core.DTOs;
using Core.MessageHandling;
using Microsoft.AspNetCore.Mvc;
using User.Query.Application.Queries;

namespace User.Query.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        
        private readonly IQueryDispatcher<UserDto> _authDispatcher;

        public AuthController(IQueryDispatcher<UserDto> authDispatcher)
        {
            _authDispatcher = authDispatcher;
        }

        [HttpGet]
        public async Task<IActionResult> LoginUser([FromQuery] string username, string password) 
        {
            var loginQuery = new LoginQuery(username, password);
            var t = await _authDispatcher.DispatchAsync(loginQuery);

            if (!t.EmailVerified)
            {
                return StatusCode(401, "Email is not verified, please verify your email by clicking on the link sent to your email address");
            }

            return StatusCode(200, t);
        }
    }
}