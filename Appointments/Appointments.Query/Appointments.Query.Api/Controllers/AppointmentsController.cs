using Appointments.Query.Application.DTOs;
using Appointments.Query.Application.Queries;
using Core.MessageHandling;
using Microsoft.AspNetCore.Mvc;

namespace Appointments.Query.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        ILogger<AppointmentsController> _logger;
        IQueryDispatcher<AppointmentsDto> _userQueryDispatcher;
        public AppointmentsController(ILogger<AppointmentsController> logger,
                                      IQueryDispatcher<AppointmentsDto> userQueryDispatcher)
        {
            _logger = logger;
            _userQueryDispatcher = userQueryDispatcher;
        }

        [HttpGet]
        [Route("circle/{id}")]
        public async Task<IActionResult> GetAppointmentsByCircleId([FromRoute] Guid id)
        {
            try
            {
                var appointmentsDto = await _userQueryDispatcher.DispatchAsync(
                    new GetAppointmentsByCircleIdQuery
                    {
                        CircleId = id
                    }
                );
                return StatusCode(200, appointmentsDto);
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message}\n{e.StackTrace}");
                return StatusCode(500, "Something went wrong, please contact support using support page.");
            }

        }

        [HttpGet]
        [Route("user/{id}")]
        public async Task<IActionResult> GetAppointmentsByUserId([FromRoute] Guid id)
        {
            try
            {
                var appointmentsDto = await _userQueryDispatcher.DispatchAsync(
                    new GetAppointmentsByUserIdQuery
                    {
                        UserId = id
                    }
                );
                return StatusCode(200, appointmentsDto);
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message}\n{e.StackTrace}");
                return StatusCode(500, "Something went wrong, please contact support using support page.");
            }
        }
    }
}