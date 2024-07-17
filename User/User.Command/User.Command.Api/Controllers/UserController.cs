using Core.MessageHandling;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using User.Command.Application.Commands;

namespace User.Command.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMessageDispatcher _dispatcher;
        private readonly ILogger<UserController> _logger;

        public UserController(IMessageDispatcher dispatcher, ILogger<UserController> logger)
        {
            _dispatcher = dispatcher;
            _logger = logger;
        }

        [Route("password")]
        [HttpPut]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordCommand command) 
        {
            var response = await _dispatcher.DispatchAsync(command);
            return StatusCode(response.ResponseCode, response.Data);
        }

        [Route("password-id-link")]
        [HttpPut]
        public async Task<IActionResult> UpdatePasswordIdLink(UpdatePasswordCommand command) 
        {
            if (command.IdLink == null)
            {
                _logger.LogWarning("Invalid id-link request. Command: {Command}", command);
                return BadRequest("Invalid request.");
            }
            
            var response = await _dispatcher.DispatchAsync(command);
            return StatusCode(response.ResponseCode, response.Data);

        }

        [Route("password/reset")]
        [HttpPut]
        public async Task<IActionResult> ResetPassword(ResetPasswordCommand command) 
        {
             var response = await _dispatcher.DispatchAsync(command);
            return StatusCode(response.ResponseCode, response.Data);
        }

        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserCommand command) 
        {
            var response = await _dispatcher.DispatchAsync(command);

            if (response.ResponseCode < 300)
            {
                return Ok(response.Data);
            } 
            else
            {
                return StatusCode(response.ResponseCode, response.Message);
            }
        }

        [Route("{id}")]
        [HttpPatch]
        public async Task<IActionResult> UpdateUser([FromRoute] Guid id, [FromBody] JsonPatchDocument jDoc)
        {
            var command = new EditUserCommand()
            {
                Id = id,
                UpdateJson = jDoc
            };
            var response = await _dispatcher.DispatchAsync(command);

            if (response.ResponseCode < 300)
            {
                return Ok(response.Data);
            } 
            else
            {
                return StatusCode(response.ResponseCode, response.Message);
            }
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser([FromRoute] string id, [FromQuery] string password)
        {
            var command = new DeleteUserCommand(Guid.Parse(id), password);
            var response = await _dispatcher.DispatchAsync(command);

            return StatusCode(response.ResponseCode, response.Message);
        }

        [Route("verify-email/{idLink}")]
        [HttpPost]
        public async Task<IActionResult> ValidateEmail([FromRoute] string idLink) 
        {
            var command = new VerifyEmailCommand(idLink); 
            var response = await _dispatcher.DispatchAsync(command);

            return StatusCode(response.ResponseCode, response.Message);
        }
    }
}