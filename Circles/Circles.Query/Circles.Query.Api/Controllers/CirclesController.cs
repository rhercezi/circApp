using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Circles.Query.Application.Queries;
using Core.DTOs;
using Core.MessageHandling;
using Microsoft.AspNetCore.Mvc;

namespace Circles.Query.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CirclesController : ControllerBase
    {
        private readonly ILogger<CirclesController> _logger;
        private readonly IMessageDispatcher _dispatcher;
        public CirclesController(ILogger<CirclesController> logger, IMessageDispatcher dispatcher)
        {
            _logger = logger;
            _dispatcher = dispatcher;
        }

        [HttpGet]
        [Route("{userId}")]
        public async Task<IActionResult> GetCirclesByUserId([FromRoute] Guid userId)
        {
            try
            {
                var response = await _dispatcher.DispatchAsync(
                    new GetCirclesByUserIdQuery
                    {
                        UserId = userId
                    }
                );

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

        [HttpGet]
        [Route("users/{circleId}")]
        public async Task<IActionResult> GetUsersByCircleId([FromRoute] Guid circleId)
        {
            try
            {
                var response = await _dispatcher.DispatchAsync(
                    new GetUsersByCircleIdQuery
                    {
                        CircleId = circleId
                    }
                );

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

        [HttpGet]
        [Route("search/{qWord}")]
        public async Task<IActionResult> SearchUsers([FromRoute] string qWord)
        {
            try
            {
                var response = await _dispatcher.DispatchAsync(
                    new SearchQuery
                    {
                        QWord = qWord
                    }
                );
                
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