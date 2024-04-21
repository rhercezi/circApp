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
        ILogger<CirclesController> _logger;
        IQueryDispatcher<AppUserDto> _userQueryDispatcher;
        IQueryDispatcher<CircleDto> _circleQueryDispatcher;
        IQueryDispatcher<AppUsersDto> _searchQueryDispatcher;
        public CirclesController(ILogger<CirclesController> logger,
                                 IQueryDispatcher<AppUserDto> userQueryDispatcher,
                                 IQueryDispatcher<CircleDto> circleQueryDispatcher,
                                 IQueryDispatcher<AppUsersDto> searchQueryDispatcher)
        {
            _logger = logger;
            _userQueryDispatcher = userQueryDispatcher;
            _circleQueryDispatcher = circleQueryDispatcher;
            _searchQueryDispatcher = searchQueryDispatcher;
        }

        [HttpGet]
        [Route("{userId}")]
        public async Task<IActionResult> GetCirclesByUserId([FromRoute] Guid userId)
        {
            AppUserDto appUserDto = new();
            try
            {
                appUserDto = await _userQueryDispatcher.DispatchAsync(
                    new GetCirclesByUserIdQuery
                    {
                        UserId = userId
                    }
                );
                return StatusCode(200, appUserDto);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return StatusCode(500, "Something went wrong, please contact support using support page.");
            }
        }

        [HttpGet]
        [Route("Users/{circleId}")]
        public async Task<IActionResult> GetUsersByCircleId([FromRoute] Guid circleId)
        {
            CircleDto circleDto = new();
            try
            {
                circleDto = await _circleQueryDispatcher.DispatchAsync(
                    new GetUsersByCircleIdQuery
                    {
                        CircleId = circleId
                    }
                );
                return StatusCode(200, circleDto);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return StatusCode(500, "Something went wrong, please contact support using support page.");
            }
        }

        [HttpGet]
        [Route("Search/{qWord}")]
        public async Task<IActionResult> SearchUsers([FromRoute] string qWord)
        {
            AppUsersDto appUsersDto = new();
            try
            {
                appUsersDto = await _searchQueryDispatcher.DispatchAsync(
                    new SearchQuery
                    {
                        QWord = qWord
                    }
                );
                return StatusCode(200, appUsersDto);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return StatusCode(500, "Something went wrong, please contact support using support page.");
            }

        }
    }
}