using Core.MessageHandling;
using Microsoft.AspNetCore.Mvc;
using User.Command.Api.Commands;
using User.Command.Application.Commands;

namespace User.Command.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            var t = await _dispatcher.DispatchAsync(command);

            return StatusCode(t.code, t.message);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserCommand command) 
        {
            var t = await _dispatcher.DispatchAsync(command);

            return StatusCode(t.code, t.message);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateUser([FromBody] EditUserCommand command)
        {
            var t = await _dispatcher.DispatchAsync(command);

            return StatusCode(t.code, t.message);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUserCommand command)
        {
            var t = await _dispatcher.DispatchAsync(command);

            return StatusCode(t.code, t.message);
        }

        [Route("VerifyEmail")]
        [HttpPost]
        public async Task<IActionResult> ValidateEmail(VerifyEmailCommand command) 
        {
            var t = await _dispatcher.DispatchAsync(command);

            return StatusCode(t.code, t.message);
        }
    }
}