using System.Reflection;
using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using User.Query.Application.DTOs;
using User.Query.Application.Exceptions;

namespace User.Query.Application.Dispatchers
{
    public class AuthDispatcher : IQueryDispatcher<LoginDto>
    {
        private Dictionary<Type, Type> _handlers = new();
        private readonly ILogger<AuthDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHandlerService _handlerService;

        public AuthDispatcher(ILogger<AuthDispatcher> logger, IServiceProvider serviceProvider, IHandlerService handlerService)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _handlerService = handlerService;
        }

        public async Task<LoginDto> DispatchAsync(BaseQuery query)
        {
            _handlers = _handlerService.RegisterHandler<BaseQuery, LoginDto>(query, Assembly.GetExecutingAssembly(), typeof(LoginDto));

            if (_handlers.TryGetValue(query.GetType(), out Type? handlerType))
            {
                try
                {
                    var handler = (IQueryHandler)_serviceProvider.GetRequiredService(handlerType);
                    return (LoginDto)await handler.HandleAsync(query);
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
            _logger.LogError($"Could not find handler for query. Type: {query.GetType().Name}");
            return new LoginDto();
        }
    }
}