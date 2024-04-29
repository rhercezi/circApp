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
        [Route("Circle/{id}")]
        public async Task<IActionResult> GetAppointmentsByCircleId([FromRoute] Guid id,
                                                                   [FromQuery] DateTime dateFrom,
                                                                   [FromQuery] DateTime dateTo)
        {
            try
            {
                var appointmentsDto = await _userQueryDispatcher.DispatchAsync(
                    new GetAppointmentsByCircleIdQuery
                    {
                        CircleId = id,
                        DareFrom = dateFrom,
                        DateTo = dateTo
                    }
                );
                return StatusCode(200, appointmentsDto);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return StatusCode(500, "Something went wrong, please contact support using support page.");
            }

        }

        [HttpGet]
        [Route("User/{id}")]
        public async Task<IActionResult> GetAppointmentsByUserId([FromRoute] Guid id,
                                                                 [FromQuery] DateTime dateFrom,
                                                                 [FromQuery] DateTime dateTo)
        {
            try
            {
                var appointmentsDto = await _userQueryDispatcher.DispatchAsync(
                    new GetAppointmentsByUserIdQuery
                    {
                        UserId = id,
                        DateFrom = dateFrom,
                        DateTo = dateTo
                    }
                );
                return StatusCode(200, appointmentsDto);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return StatusCode(500, "Something went wrong, please contact support using support page.");
            }
        }
    }
}