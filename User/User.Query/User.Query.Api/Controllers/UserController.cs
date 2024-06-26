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

        [Route("by-id/{id}")]
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
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return StatusCode(400, "Something went wrong, please contact support using support page.");
            }

            return StatusCode(200, user);
        }

        [Route("by-email")]
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
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return StatusCode(400, "Something went wrong, please contact support using support page.");
            }

            return StatusCode(200, user);
        }

        [Route("by-username")]
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
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return StatusCode(400, "Something went wrong, please contact support using support page.");
            }

            return StatusCode(200, user);
        }
    }
}