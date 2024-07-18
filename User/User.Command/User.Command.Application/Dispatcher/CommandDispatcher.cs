using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using User.Command.Application.Exceptions;
using User.Command.Domain.Exceptions;

namespace User.Command.Application.Dispatcher
{
    public class CommandDispatcher : IMessageDispatcher
    {
        private readonly ILogger<CommandDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;

        public CommandDispatcher(ILogger<CommandDispatcher> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task<BaseResponse> DispatchAsync(BaseMessage message)
        {
            try
            {
                Type genericType = typeof(IMessageHandler<>).MakeGenericType(message.GetType());
                var handler = (IMessageHandler)_serviceProvider.GetRequiredService(genericType);
                return await handler.HandleAsync(message);
            }
            catch (UserValidationException e)
            {
                _logger.LogWarning(e.Message);
                return new BaseResponse { ResponseCode = 422, Message = e.Message };
            }
            catch (UserDomainException e)
            {
                _logger.LogWarning(e.Message);
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