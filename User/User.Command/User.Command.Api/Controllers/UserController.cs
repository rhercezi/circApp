using Core.MessageHandling;
using Microsoft.AspNetCore.Mvc;
using User.Command.Api.Commands;
using User.Command.Application.Commands;

namespace User.Command.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ICommandDispatcher _dispatcher;
        private readonly ILogger<UserController> _logger;

        public UserController(ICommandDispatcher dispatcher, ILogger<UserController> logger)
        {
            _dispatcher = dispatcher;
            _logger = logger;
        }

        [Route("password")]
        [HttpPatch]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordCommand command) 
        {
            var (code, message) = await _dispatcher.DispatchAsync(command);

            return StatusCode(code, message);
        }

        [Route("password-id-link")]
        [HttpPatch]
        public async Task<IActionResult> UpdatePasswordIdLink(UpdatePasswordCommand command) 
        {
            if (command.IdLink == null)
            {
                _logger.LogWarning("Invalid id-link request. Command: {Command}", command);
                return BadRequest("Invalid request.");
            }
            var (code, message) = await _dispatcher.DispatchAsync(command);
            return StatusCode(code, message);

        }

        [Route("password/reset")]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordCommand command) 
        {
            var (code, message) = await _dispatcher.DispatchAsync(command);

            return StatusCode(code, message);
        }

        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserCommand command) 
        {
            var (code, message) = await _dispatcher.DispatchAsync(command);

            return StatusCode(code, message);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateUser([FromBody] EditUserCommand command)
        {
            var (code, message) = await _dispatcher.DispatchAsync(command);

            return StatusCode(code, message);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser([FromQuery] string id, [FromQuery] string password)
        {
            var command = new DeleteUserCommand(Guid.Parse(id), password);
            var (code, message) = await _dispatcher.DispatchAsync(command);

            return StatusCode(code, message);
        }

        [Route("verify-email/{idLink}")]
        [HttpPost]
        public async Task<IActionResult> ValidateEmail([FromRoute] string idLink) 
        {
            var command = new VerifyEmailCommand(idLink); 
            var (code, message) = await _dispatcher.DispatchAsync(command);

            return StatusCode(code, message);
        }
    }
}