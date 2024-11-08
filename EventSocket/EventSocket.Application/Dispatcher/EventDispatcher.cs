using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EventSocket.Application.Dispatcher
{
    public class EventDispatcher : IMessageDispatcher
    {
        private readonly ILogger<EventDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;

        public EventDispatcher(IServiceProvider serviceProvider, ILogger<EventDispatcher> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task<BaseResponse> DispatchAsync(BaseMessage message)
        {
            try
            {
                Type genericType = typeof(IMessageHandler<>).MakeGenericType(message.GetType());
                var handler = (IMessageHandler)_serviceProvider.GetRequiredService(genericType);
                return await handler.HandleAsync(message);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return new BaseResponse { ResponseCode = 500, Message = "Something went wrong, please try again later." };
            }
        }
    }
}