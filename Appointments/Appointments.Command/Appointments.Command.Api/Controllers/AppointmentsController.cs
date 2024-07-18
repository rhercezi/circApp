using Appointments.Command.Application.Commands;
using Core.MessageHandling;
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
                return StatusCode(500, "Something went wrong, please try again later.");
            }
        }

        [Route("{appointmentId}")]
        [HttpPatch]
        public async Task<IActionResult> UpdateAppointment([FromRoute] Guid appointmentId, [FromBody] JsonPatchDocument patchDocument)
        {
            var command = new UpdateAppointmentCommand(Guid.Parse(Request.Headers["userId"].ToString()), appointmentId, patchDocument);
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
                return StatusCode(500, "Something went wrong, please try again later.");
            }
        }

        [Route("{appointmentId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteAppointment([FromRoute] Guid appointmentId)
        {
            try
            {
                var command = new DeleteAppointmentCommand
                {
                    UserId = Guid.Parse(Request.Headers["userId"].ToString()),
                    AppointmentId = appointmentId
                };

                var response= await _dispatcher.DispatchAsync(command);
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
                return StatusCode(500, "Something went wrong, please try again later.");
            }
        }

        [Route("details/{appointmentId}")]
        [HttpPatch]
        public async Task<IActionResult> UpdateAppointmentDetails([FromRoute] Guid appointmentId, [FromBody] JsonPatchDocument patchDocument)
        {
            var command = new UpdateAppointmentDetailCommand(appointmentId, Guid.Parse(Request.Headers["userId"].ToString()), patchDocument);
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
                return StatusCode(500, "Something went wrong, please try again later.");
            }
        }

        [Route("details/{appointmentId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteAppointmentDetails([FromRoute] Guid appointmentId)
        {
            try
            {
                var command = new DeleteAppointmentDetailCommand
                {
                    UserId = Guid.Parse(Request.Headers["userId"].ToString()),
                    AppointmentId = appointmentId
                };

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
                return StatusCode(500, "Something went wrong, please try again later.");
            }
        }
    }
}