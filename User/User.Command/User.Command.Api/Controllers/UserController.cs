using Core.MessageHandling;
using Microsoft.AspNetCore.Mvc;
using User.Command.Api.Commands;

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

        [HttpGet]
        public async Task<IActionResult> GetUser(CreateUserCommand command) 
        {

            
            await _dispatcher.DispatchAsync(command);

            return Ok("{OK}");
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserCommand command) 
        {
            await _dispatcher.DispatchAsync(command);

            return Ok("{OK}");
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateUser([FromBody] EditUserCommand command)
        {
            await _dispatcher.DispatchAsync(command);

            return Ok("{OK}");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUserCommand command)
        {
            await _dispatcher.DispatchAsync(command);

            return Ok("{OK}");
        }
    }
}