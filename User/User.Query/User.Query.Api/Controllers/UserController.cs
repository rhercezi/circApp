using Core.DTOs;
using Core.MessageHandling;
using Microsoft.AspNetCore.Mvc;
using User.Query.Application.Queries;

namespace User.Query.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IQueryDispatcher<UserDto> _queryDispatcher;

        public UserController(IQueryDispatcher<UserDto> dispatcher)
        {
            _queryDispatcher = dispatcher;
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
    }
}