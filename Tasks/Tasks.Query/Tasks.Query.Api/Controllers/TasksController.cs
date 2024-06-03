using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.DTOs;
using Core.MessageHandling;
using Microsoft.AspNetCore.Mvc;
using Tasks.Query.Application.Queries;

namespace Tasks.Query.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly IQueryDispatcher<TasksDto> _queryDispatcher;
        private readonly ILogger<TasksController> _logger;

        public TasksController(IQueryDispatcher<TasksDto> queryDispatcher,
                               ILogger<TasksController> logger)
        {
            _queryDispatcher = queryDispatcher;
            _logger = logger;
        }

        [HttpGet("circle/{circleId}")]
        public async Task<IActionResult> GetTasksForCircleAsync([FromRoute] Guid circleId)
        {
            try
            {
                var query = new GetTasksForCircleQuery { CircleId = circleId };
                var tasks = await _queryDispatcher.DispatchAsync(query);
                return Ok(tasks);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return StatusCode(500, "Something went wrong, please try again later.");
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetTasksForUserAsync([FromRoute] Guid userId, [FromQuery] bool searchByCircles)
        {
            try
            {
                var query = new GetTasksForUserQuery { UserId = userId, SearchByCircles = searchByCircles };
                var tasks = await _queryDispatcher.DispatchAsync(query);
                return Ok(tasks);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return StatusCode(500, "Something went wrong, please try again later.");
            }
        }
    }
}