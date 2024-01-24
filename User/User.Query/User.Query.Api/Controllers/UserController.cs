using Core.MessageHandling;
using Microsoft.AspNetCore.Mvc;
using User.Query.Application.DTOs;
using User.Query.Application.Queries;

namespace User.Query.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IQueryDispatcher<UserDto> _dispatcher;

        public UserController(IQueryDispatcher<UserDto> dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [Route("ById")]
        [HttpGet]
        public async Task<IActionResult> GetUserById(GetUserByIdQuery query) 
        {
            var t = await _dispatcher.DispatchAsync(query);

            return StatusCode(200, t);
        }

        [Route("ByEmail")]
        [HttpGet]
        public async Task<IActionResult> GetUserByEmail(GetUserByEmailQuery query) 
        {
            var t = await _dispatcher.DispatchAsync(query);

            return StatusCode(200, t);
        }

        [Route("ByUsername")]
        [HttpGet]
        public async Task<IActionResult> GetUserByUsername(GetUserByUsernameQuery query) 
        {
            var t = await _dispatcher.DispatchAsync(query);

            return StatusCode(200, t);
        }

        [Route("GetToken")]
        [HttpGet]
        public async Task<IActionResult> GetToken(LoginQuery loginDto) 
        {
            var t = await _dispatcher.DispatchAsync(loginDto);

            return StatusCode(200, t);
        }

    }
}