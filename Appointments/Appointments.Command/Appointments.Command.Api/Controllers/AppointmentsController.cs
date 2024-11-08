using Appointments.Command.Application.Commands;
using Core.MessageHandling;
using Core.Utilities;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Appointments.Command.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IMessageDispatcher _dispatcher;
        private readonly ILogger<AppointmentsController> _logger;
        public AppointmentsController(IMessageDispatcher dispatcher, ILogger<AppointmentsController> logger)
        {
            _dispatcher = dispatcher;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment(CreateAppointmentCommand command)
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
                return StatusCode(500, "Something went wrong, please try again later.");
            }
        }

        [Route("{appointmentId}")]
        [HttpPatch]
        public async Task<IActionResult> UpdateAppointment([FromRoute] Guid appointmentId, [FromBody] JsonPatchDocument patchDocument)
        {
            var accessToken = Request.Cookies["AccessToken"];
            if (string.IsNullOrEmpty(accessToken))
            {
                return StatusCode(401, "Access token is missing or invalid.");
            }
            var ownerId = Guid.Parse(JwtService.GetClaimValue(accessToken, "sub"));
            var command = new UpdateAppointmentCommand(ownerId, appointmentId, patchDocument);
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
                return StatusCode(500, "Something went wrong, please try again later.");
            }
        }

        [Route("{appointmentId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteAppointment([FromRoute] Guid appointmentId)
        {
            try
            {
                var accessToken = Request.Cookies["AccessToken"];
                if (string.IsNullOrEmpty(accessToken))
                {
                    return StatusCode(401, "Access token is missing or invalid.");
                }
                var userId = Guid.Parse(JwtService.GetClaimValue(accessToken, "sub"));

                var command = new DeleteAppointmentCommand
                {
                    UserId = userId,
                    AppointmentId = appointmentId
                };

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
                return StatusCode(500, "Something went wrong, please try again later.");
            }
        }

        [Route("details")]
        [HttpPost]
        public async Task<IActionResult> CreateAppointmentDetails(AddAppointmentDetailsCommand command)
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
                return StatusCode(500, "Something went wrong, please try again later.");
            }
        }

        [Route("details/{appointmentId}")]
        [HttpPatch]
        public async Task<IActionResult> UpdateAppointmentDetails([FromRoute] Guid appointmentId, [FromBody] JsonPatchDocument patchDocument)
        {
            var accessToken = Request.Cookies["AccessToken"];
            if (string.IsNullOrEmpty(accessToken))
            {
                return StatusCode(401, "Access token is missing or invalid.");
            }
            var ownerId = Guid.Parse(JwtService.GetClaimValue(accessToken, "sub"));

            var command = new UpdateAppointmentDetailCommand(ownerId, appointmentId, patchDocument);
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
                return StatusCode(500, "Something went wrong, please try again later.");
            }
        }

        [Route("details/{appointmentId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteAppointmentDetails([FromRoute] Guid appointmentId)
        {
            try
            {
                var accessToken = Request.Cookies["AccessToken"];
                if (string.IsNullOrEmpty(accessToken))
                {
                    return StatusCode(401, "Access token is missing or invalid.");
                }
                var userId = Guid.Parse(JwtService.GetClaimValue(accessToken, "sub"));

                var command = new DeleteAppointmentDetailCommand
                {
                    UserId = userId,
                    AppointmentId = appointmentId
                };

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
                return StatusCode(500, "Something went wrong, please try again later.");
            }
        }
    }
}