using Core.Utilities;
using EventSocket.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventSocket.Api.Controllers
{
    [ApiController]
    [Route("api/v1/event-socket")]
    public class EventSocketController : ControllerBase
    {
        private readonly SocketConnectionManager _socketConnectionManager;
        private readonly ILogger<EventSocketController> _logger;

        public EventSocketController(SocketConnectionManager socketConnectionManager, ILogger<EventSocketController> logger)
        {
            _socketConnectionManager = socketConnectionManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                _logger.LogDebug("WebSocket request received.");
                try
                {
                    using var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                    var accessToken = Request.Cookies["AccessToken"];
                    if (string.IsNullOrEmpty(accessToken))
                    {
                        return StatusCode(401, "Access token is missing or invalid.");
                    }
                    var userId = Guid.Parse(JwtService.GetClaimValue(accessToken, "sub"));
                    await _socketConnectionManager.AddSocketAsync(userId, socket);

                    _logger.LogDebug("WebSocket connection established: {UserId}", userId);
                    return StatusCode(101, "Switching Protocols");
                }
                catch (Exception e)
                {
                    _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                }
                return StatusCode(500, "Internal Server Error");
            }
            else
            {
                _logger.LogDebug("Invalid request received: {Request}", HttpContext.Request.ToString());
                return StatusCode(400, "Bad Request");
            }
        }
    }
}