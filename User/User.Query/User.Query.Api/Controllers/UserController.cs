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
        private readonly IQueryDispatcher<UserDto> _queryDispatcher;
        private readonly IQueryDispatcher<TokenDto> _authDispatcher;

        public UserController(IQueryDispatcher<UserDto> dispatcher, IQueryDispatcher<TokenDto> authDispatcher)
        {
            _queryDispatcher = dispatcher;
            _authDispatcher = authDispatcher;
        }

        [Route("ById/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetUserById([FromRoute] Guid id) 
        {
            var query = new GetUserByIdQuery{Id = id};
            var t = await _queryDispatcher.DispatchAsync(query);

            return StatusCode(200, t);
        }

        [Route("ByEmail")]
        [HttpGet]
        public async Task<IActionResult> GetUserByEmail([FromQuery] string email) 
        {
            var query = new GetUserByEmailQuery(email);
            var t = await _queryDispatcher.DispatchAsync(query);

            return StatusCode(200, t);
        }

        [Route("ByUsername")]
        [HttpGet]
        public async Task<IActionResult> GetUserByUsername([FromQuery] string username) 
        {
            var query = new GetUserByUsernameQuery(username);
            var t = await _queryDispatcher.DispatchAsync(query);

            return StatusCode(200, t);
        }

        [Route("GetToken")]
        [HttpGet]
        public async Task<IActionResult> GetToken([FromQuery] string username, string password) 
        {
            var loginQuery = new LoginQuery(username, password);
            var t = await _authDispatcher.DispatchAsync(loginQuery);

            return StatusCode(200, t);
        }

    }
}