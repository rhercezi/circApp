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
            try
            {
                var response = await _dispatcher.DispatchAsync(command);
                if (response.ResponseCode < 500)
                {
                    return StatusCode(response.ResponseCode, response.Data);
                }
                else
                {
                    return StatusCode(response.ResponseCode, "Something went wrong, please contact support using support page.");
                }
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return StatusCode(500, "Something went wrong, please contact support using support page.");
            }
        }

        [Route("password-id-link")]
        [HttpPut]
        public async Task<IActionResult> UpdatePasswordIdLink(UpdatePasswordCommand command)
        {
            try
            {
                if (command.IdLink == null)
                {
                    _logger.LogWarning("Invalid id-link request. Command: {Command}", command);
                    return BadRequest("Invalid request.");
                }
                var response = await _dispatcher.DispatchAsync(command);
                if (response.ResponseCode < 500)
                {
                    return StatusCode(response.ResponseCode, response.Data);
                }
                else
                {
                    return StatusCode(response.ResponseCode, "Something went wrong, please contact support using support page.");
                }
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return StatusCode(500, "Something went wrong, please contact support using support page.");
            }

        }

        [Route("password/reset")]
        [HttpPut]
        public async Task<IActionResult> ResetPassword(ResetPasswordCommand command)
        {
            try
            {
                var response = await _dispatcher.DispatchAsync(command);
                if (response.ResponseCode < 500)
                {
                    return StatusCode(response.ResponseCode, response.Data);
                }
                else
                {
                    return StatusCode(response.ResponseCode, "Something went wrong, please contact support using support page.");
                }
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return StatusCode(500, "Something went wrong, please contact support using support page.");
            }
        }

        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserCommand command)
        {
            try
            {
                var response = await _dispatcher.DispatchAsync(command);
                if (response.ResponseCode < 500)
                {
                    return StatusCode(response.ResponseCode, response.Data);
                }
                else
                {
                    return StatusCode(response.ResponseCode, "Something went wrong, please contact support using support page.");
                }
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return StatusCode(500, "Something went wrong, please contact support using support page.");
            }
        }

        [Route("{id}")]
        [HttpPatch]
        public async Task<IActionResult> UpdateUser([FromRoute] Guid id, [FromBody] JsonPatchDocument jDoc)
        {
            try
            {
                var command = new EditUserCommand()
                {
                    Id = id,
                    UpdateJson = jDoc
                };
                var response = await _dispatcher.DispatchAsync(command);
                if (response.ResponseCode < 299)
                {
                    return StatusCode(response.ResponseCode, response.Data);
                }
                if (response.ResponseCode > 299 && response.ResponseCode < 500)
                {
                    return StatusCode(response.ResponseCode, response.Message);
                }
                else
                {
                    return StatusCode(response.ResponseCode, "Something went wrong, please contact support using support page.");
                }
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return StatusCode(500, "Something went wrong, please contact support using support page.");
            }
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser([FromRoute] string id, [FromQuery] string password)
        {
            try
            {
                var command = new DeleteUserCommand(Guid.Parse(id), password);
                var response = await _dispatcher.DispatchAsync(command);
                if (response.ResponseCode < 500)
                {
                    return StatusCode(response.ResponseCode, response.Data);
                }
                else
                {
                    return StatusCode(response.ResponseCode, "Something went wrong, please contact support using support page.");
                }
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return StatusCode(500, "Something went wrong, please contact support using support page.");
            }
        }

        [Route("verify-email/{idLink}")]
        [HttpPost]
        public async Task<IActionResult> ValidateEmail([FromRoute] string idLink)
        {
            try
            {
                var command = new VerifyEmailCommand(idLink);
                var response = await _dispatcher.DispatchAsync(command);
                if (response.ResponseCode < 500)
                {
                    return StatusCode(response.ResponseCode, response.Data);
                }
                else
                {
                    return StatusCode(response.ResponseCode, "Something went wrong, please contact support using support page.");
                }
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return StatusCode(500, "Something went wrong, please contact support using support page.");
            }
        }
    }
}