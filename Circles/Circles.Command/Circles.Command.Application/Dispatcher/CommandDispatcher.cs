using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Circles.Command.Application.Dispatcher
{
    public class CommandDispatcher : IMessageDispatcher
    {
        private readonly ILogger<CommandDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;

        public CommandDispatcher(IServiceProvider serviceProvider, ILogger<CommandDispatcher> logger)
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
            catch (CirclesValidationException e)
            {
                return new BaseResponse { ResponseCode = 422, Message = e.Message };
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return new BaseResponse { ResponseCode = 500, Message = "Something went wrong, please try again later." };
            }
        }
    }
}