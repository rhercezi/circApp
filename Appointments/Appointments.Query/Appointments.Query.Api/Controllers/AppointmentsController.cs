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
        IMessageDispatcher _userQueryDispatcher;
        public AppointmentsController(ILogger<AppointmentsController> logger,
                                      IMessageDispatcher userQueryDispatcher)
        {
            _logger = logger;
            _userQueryDispatcher = userQueryDispatcher;
        }

        [HttpGet]
        [Route("circle/{id}")]
        public async Task<IActionResult> GetAppointmentsByCircleId([FromRoute] Guid id,
                                                                   [FromQuery] DateTime dateFrom,
                                                                   [FromQuery] DateTime dateTo)
        {
            try
            {
                var response = await _userQueryDispatcher.DispatchAsync(
                    new GetAppointmentsByCircleIdQuery
                    {
                        CircleId = id,
                        DateFrom = dateFrom,
                        DateTo = dateTo
                    }
                );
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
                return StatusCode(500, "Something went wrong, please contact support using support page.");
            }

        }

        [HttpGet]
        [Route("user/{id}")]
        public async Task<IActionResult> GetAppointmentsByUserId([FromRoute] Guid id,
                                                                 [FromQuery] DateTime dateFrom,
                                                                 [FromQuery] DateTime dateTo)
        {
            try
            {
                var response = await _userQueryDispatcher.DispatchAsync(
                    new GetAppointmentsByUserIdQuery
                    {
                        UserId = id,
                        DateFrom = dateFrom,
                        DateTo = dateTo
                    }
                );
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
                return StatusCode(500, "Something went wrong, please contact support using support page.");
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetAppointmentsId([FromRoute] Guid id)
        {
            try
            {
                var response = await _userQueryDispatcher.DispatchAsync(
                    new GetAppointmentByIdQuery
                    {
                        Id = id
                    }
                );
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
                return StatusCode(500, "Something went wrong, please contact support using support page.");
            }
        }
    }
}