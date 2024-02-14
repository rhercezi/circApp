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
        private readonly ILogger<UserController> _logger;

        public UserController(IQueryDispatcher<UserDto> dispatcher, ILogger<UserController> logger)
        {
            _queryDispatcher = dispatcher;
            _logger = logger;
        }

        [Route("ById/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetUserById([FromRoute] Guid id) 
        {
            var query = new GetUserByIdQuery{Id = id};
            UserDto user;
            try
            {
                user = await _queryDispatcher.DispatchAsync(query);
            }
            catch (Exception e)
            {
                _logger.LogError(e.StackTrace, e.Message);
                return StatusCode(400, "Something went wrong, please contact support using support page.");
            }

            return StatusCode(200, user);
        }

        [Route("ByEmail")]
        [HttpGet]
        public async Task<IActionResult> GetUserByEmail([FromQuery] string email) 
        {
            var query = new GetUserByEmailQuery(email);
            UserDto user;
            try
            {
                user = await _queryDispatcher.DispatchAsync(query);
            }
            catch (Exception e)
            {
                _logger.LogError(e.StackTrace, e.Message);
                return StatusCode(400, "Something went wrong, please contact support using support page.");
            }

            return StatusCode(200, user);
        }

        [Route("ByUsername")]
        [HttpGet]
        public async Task<IActionResult> GetUserByUsername([FromQuery] string username) 
        {
            var query = new GetUserByUsernameQuery(username);
            UserDto user;
            try
            {
                user = await _queryDispatcher.DispatchAsync(query);
            }
            catch (Exception e)
            {
                _logger.LogError(e.StackTrace, e.Message);
                return StatusCode(400, "Something went wrong, please contact support using support page.");
            }

            return StatusCode(200, user);
        }
    }
}