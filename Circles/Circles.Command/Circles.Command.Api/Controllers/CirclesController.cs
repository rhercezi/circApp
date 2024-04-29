using Circles.Command.Application.Commands;
using Core.MessageHandling;
using Microsoft.AspNetCore.Mvc;

namespace Circles.Command.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CirclesController : ControllerBase
    {
        private readonly ICommandDispatcher _dispatcher;
        private readonly ILogger<CirclesController> _logger;

        public CirclesController(ICommandDispatcher dispatcher, ILogger<CirclesController> logger)
        {
            _dispatcher = dispatcher;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCircle([FromBody] CreateCircleCommand command)
        {
            try
            {
                var (code, message) = await _dispatcher.DispatchAsync(command);
                return StatusCode(code, message);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return StatusCode(500, "Something went wrong, please contact support using support page.");
            }
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateCircle([FromBody] UpdateCircleCommand command)
        {
            try
            {
                var (code, message) = await _dispatcher.DispatchAsync(command);
                return StatusCode(code, message);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return StatusCode(500, "Something went wrong, please contact support using support page.");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCircle([FromBody] DeleteCircleCommand command)
        {
            try
            {
                var (code, message) = await _dispatcher.DispatchAsync(command);
                return StatusCode(code, message);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return StatusCode(500, "Something went wrong, please contact support using support page.");
            }
        }

        [HttpPost]
        [Route("Confirm")]
        public async Task<IActionResult> ConfirmJoinCircle([FromBody] ConfirmJoinCommand command)
        {
            try
            {
                var (code, message) = await _dispatcher.DispatchAsync(command);
                return StatusCode(code, message);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return StatusCode(500, "Something went wrong, please contact support using support page.");
            }
        }

        [HttpPost]
        [Route("AddUsers")]
        public async Task<IActionResult> AddUsersToCircle([FromBody] AddUsersCommand command)
        {
            try
            {
                var (code, message) = await _dispatcher.DispatchAsync(command);
                return StatusCode(code, message);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return StatusCode(500, "Something went wrong, please contact support using support page.");
            }
        }

        [HttpPost]
        [Route("RemoveUsers")]
        public async Task<IActionResult> RemoveUsersFromCircle([FromBody] RemoveUsersCommand command)
        {
            try
            {
                var (code, message) = await _dispatcher.DispatchAsync(command);
                return StatusCode(code, message);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return StatusCode(500, "Something went wrong, please contact support using support page.");
            }
        }
    }


}