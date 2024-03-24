using Appointments.Command.Application.Commands;
using Core.MessageHandling;
using Microsoft.AspNetCore.Mvc;

namespace Appointments.Command.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly ICommandDispatcher _dispatcher;
        private readonly ILogger<AppointmentsController> _logger;
        public AppointmentsController(ICommandDispatcher dispatcher, ILogger<AppointmentsController> logger)
        {
            _dispatcher = dispatcher;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment(CreateAppointmentCommand command)
        {
            try
            {
                var (code, message) = await _dispatcher.DispatchAsync(command);
                return StatusCode(code, message);
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message}\n{e.StackTrace}");
                return StatusCode(500, "Something went wrong, please try again later.");
            }
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateAppointment(UpdateAppointmentCommand command)
        {
            try
            {
                var (code, message) = await _dispatcher.DispatchAsync(command);
                return StatusCode(code, message);
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message}\n{e.StackTrace}");
                return StatusCode(500, "Something went wrong, please try again later.");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> CreateAppointment(DeleteAppointmentCommand command)
        {
            try
            {
                var (code, message) = await _dispatcher.DispatchAsync(command);
                return StatusCode(code, message);
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message}\n{e.StackTrace}");
                return StatusCode(500, "Something went wrong, please try again later.");
            }
        }

        [Route("details")]
        [HttpPost]
        public async Task<IActionResult> CreateAppointmentDetails(AddAppointmentDetailsCommand command)
        {
            try
            {
                var (code, message) = await _dispatcher.DispatchAsync(command);
                return StatusCode(code, message);
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message}\n{e.StackTrace}");
                return StatusCode(500, "Something went wrong, please try again later.");
            }
        }

        [Route("details")]
        [HttpPatch]
        public async Task<IActionResult> UpdateAppointmentDetails(UpdateAppointmentDetailCommand command)
        {
            try
            {
                var (code, message) = await _dispatcher.DispatchAsync(command);
                return StatusCode(code, message);
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message}\n{e.StackTrace}");
                return StatusCode(500, "Something went wrong, please try again later.");
            }
        }

        [Route("details")]
        [HttpDelete]
        public async Task<IActionResult> CreateAppointmentDetails(DeleteAppointmentDetailCommand command)
        {
            try
            {
                var (code, message) = await _dispatcher.DispatchAsync(command);
                return StatusCode(code, message);
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message}\n{e.StackTrace}");
                return StatusCode(500, "Something went wrong, please try again later.");
            }
        }
    }
}