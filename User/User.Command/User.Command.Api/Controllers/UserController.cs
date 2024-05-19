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

        public UserController(ICommandDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [Route("Password")]
        [HttpPatch]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordCommand command) 
        {
            var (code, message) = await _dispatcher.DispatchAsync(command);

            return StatusCode(code, message);
        }

        [Route("Password/Reset")]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordCommand command) 
        {
            var (code, message) = await _dispatcher.DispatchAsync(command);

            return StatusCode(code, message);
        }

        [Route("Create")]
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
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUserCommand command)
        {
            var (code, message) = await _dispatcher.DispatchAsync(command);

            return StatusCode(code, message);
        }

        [Route("VerifyEmail/{idLink}")]
        [HttpPost]
        public async Task<IActionResult> ValidateEmail([FromRoute] string idLink) 
        {
            var command = new VerifyEmailCommand(idLink); 
            var (code, message) = await _dispatcher.DispatchAsync(command);

            return StatusCode(code, message);
        }
    }
}