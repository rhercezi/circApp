using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using User.Query.Application.Exceptions;

namespace User.Query.Application.Dispatchers
{
    public class QueryDispatcher : IMessageDispatcher
    {
        private readonly ILogger<QueryDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;

        public QueryDispatcher(ILogger<QueryDispatcher> logger, IServiceProvider serviceProvider)
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
                catch (AuthException e)
                {
                    _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                    throw e;
                }
                catch (Exception e)
                {
                    _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                    throw new QueryApplicationException("Auth dispatcher failed", e);
                }
        }
    }
}