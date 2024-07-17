using Circles.Command.Application.Commands;
using Circles.Command.Application.DTOs;
using Circles.Domain.Entities;
using Core.MessageHandling;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Circles.Command.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CirclesController : ControllerBase
    {
        private readonly IMessageDispatcher _dispatcher;
        private readonly ILogger<CirclesController> _logger;

        public CirclesController(IMessageDispatcher dispatcher, ILogger<CirclesController> logger)
        {
            _dispatcher = dispatcher;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCircle([FromBody] CreateCircleCommand command)
        {
            try
            {
                var response = await _dispatcher.DispatchAsync(command);
                if (response.ResponseCode < 300)
                {
                    return StatusCode(response.ResponseCode, response.Data);
                }
                else if (response.ResponseCode < 500)
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
        [HttpPatch]
        public async Task<IActionResult> UpdateCircle([FromRoute] Guid id ,[FromBody] JsonPatchDocument jsonPatch)
        {
            try
            {
                var response = await _dispatcher.DispatchAsync(new UpdateCircleCommand { CircleId = id, JsonPatchDocument = jsonPatch });
                if (response.ResponseCode < 300)
                {
                    return StatusCode(response.ResponseCode, response.Data);
                }
                else if (response.ResponseCode < 500)
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
                return StatusCode(500, "Something went wrong, please contact support using support page");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCircle([FromQuery] string id)
        {
            try
            {
                var command = new DeleteCircleCommand { Id = Guid.Parse(id) };
                var response = await _dispatcher.DispatchAsync(command);
                if (response.ResponseCode < 300)
                {
                    return StatusCode(response.ResponseCode, response.Data);
                }
                else if (response.ResponseCode < 500)
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

        [HttpPost]
        [Route("confirm")]
        public async Task<IActionResult> ConfirmJoinCircle([FromBody] ConfirmJoinCommand command)
        {
            try
            {
                var response = await _dispatcher.DispatchAsync(command);
                if (response.ResponseCode < 300)
                {
                    return StatusCode(response.ResponseCode, response.Data);
                }
                else if (response.ResponseCode < 500)
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

        [HttpPost]
        [Route("add-users")]
        public async Task<IActionResult> AddUsersToCircle([FromBody] AddUsersCommand command)
        {
            try
            {
                var response = await _dispatcher.DispatchAsync(command);
                if (response.ResponseCode < 300)
                {
                    return StatusCode(response.ResponseCode, response.Data);
                }
                else if (response.ResponseCode < 500)
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

        [HttpPost]
        [Route("remove-users")]
        public async Task<IActionResult> RemoveUsersFromCircle([FromBody] RemoveUsersCommand command)
        {
            try
            {
                var response = await _dispatcher.DispatchAsync(command);
                if (response.ResponseCode < 300)
                {
                    return StatusCode(response.ResponseCode, response.Data);
                }
                else if (response.ResponseCode < 500)
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
    }
}