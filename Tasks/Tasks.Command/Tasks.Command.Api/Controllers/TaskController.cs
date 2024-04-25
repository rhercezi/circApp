using Core.MessageHandling;
using Microsoft.AspNetCore.Mvc;
using Tasks.Command.Application.Commands;

namespace Tasks.Command.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ICommandDispatcher commandDispatcher, ILogger<TasksController> logger)
        {
            _commandDispatcher = commandDispatcher;
            _logger = logger;
        }
        [HttpPost]
        public async Task<IActionResult> CreateTaskAsync([FromBody] CreateTaskCommand command)
        {
            try
            {
                (var code, var message) = await _commandDispatcher.DispatchAsync(command);
                return StatusCode(code, message);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return StatusCode(500, "Something went wrong, please try again later.");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTaskAsync([FromBody] UpdateTaskCommand command)
        {
            try
            {
                var ownerId = Guid.Parse(Request.Headers["ownerId"].ToString());
                if (command.OwnerId != ownerId)
                {
                    return StatusCode(403, "You are not authorized to perform this action.");
                }
                (var code, var message) = await _commandDispatcher.DispatchAsync(command);
                return StatusCode(code, message);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return StatusCode(500, "Something went wrong, please try again later.");
            }
        }

        [HttpPatch]
        public async Task<IActionResult> CompleteTaskAsync([FromBody] CompleteTaskCommand command)
        {
            try
            {
                if (command.UserId != Guid.Empty)
                {
                    var userId = Guid.Parse(Request.Headers["ownerId"].ToString());
                    if (command.UserId != userId)
                    {
                        return StatusCode(403, "You are not authorized to perform this action.");
                    }
                }
                (var code, var message) = await _commandDispatcher.DispatchAsync(command);
                return StatusCode(code, message);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return StatusCode(500, "Something went wrong, please try again later.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskAsync([FromRoute] Guid id)
        {
            try
            {
                var ownerId = Guid.Parse(Request.Headers["ownerId"].ToString());
                (var code, var message) = await _commandDispatcher.DispatchAsync(new DeleteTaskCommand { Id = id, OwnerId = ownerId});
                return StatusCode(code, message);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return StatusCode(500, "Something went wrong, please try again later.");
            }
        }
    }
}