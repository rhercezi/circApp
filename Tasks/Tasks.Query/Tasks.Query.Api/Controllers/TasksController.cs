using Core.MessageHandling;
using Microsoft.AspNetCore.Mvc;
using Tasks.Query.Application.Queries;

namespace Tasks.Query.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly IMessageDispatcher _queryDispatcher;
        private readonly ILogger<TasksController> _logger;

        public TasksController(IMessageDispatcher queryDispatcher,
                               ILogger<TasksController> logger)
        {
            _queryDispatcher = queryDispatcher;
            _logger = logger;
        }

        [HttpGet("circle/{circleId}")]
        public async Task<IActionResult> GetTasksForCircleAsync([FromRoute] Guid circleId, [FromQuery] bool IncludeCompleted)
        {
            try
            {
                var query = new GetTasksForCircleQuery { CircleId = circleId, IncludeCompleted = IncludeCompleted };
                var response = await _queryDispatcher.DispatchAsync(query);
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
                return StatusCode(500, "Something went wrong, please try again later.");
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetTasksForUserAsync([FromRoute] Guid userId,
                                                              [FromQuery] bool searchByCircles,
                                                              [FromQuery] bool includeCompleted)
        {
            try
            {
                var query = new GetTasksForUserQuery
                {
                    UserId = userId,
                    SearchByCircles = searchByCircles,
                    IncludeCompleted = includeCompleted
                };
                var response = await _queryDispatcher.DispatchAsync(query);
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
                return StatusCode(500, "Something went wrong, please try again later.");
            }
        }

        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetTaskAsync([FromRoute] Guid taskId)
        {
            try
            {
                var query = new GetTaskQuery { TaskId = taskId };
                var response = await _queryDispatcher.DispatchAsync(query);
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
                return StatusCode(500, "Something went wrong, please try again later.");
            }
        }
    }
}