using System.Reflection;
using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using User.Query.Application.Exceptions;

namespace User.Query.Application.Dispatchers
{
    public class QueryDispatcher : IQueryDispatcher<UserDto>
    {
        private Dictionary<Type, Type> _handlers = new();
        private readonly ILogger<QueryDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHandlerService _handlerService;

        public QueryDispatcher(ILogger<QueryDispatcher> logger, IServiceProvider serviceProvider, IHandlerService handlerService)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _handlerService = handlerService;
        }

        public async Task<UserDto> DispatchAsync(BaseQuery query)
        {
            _handlers = _handlerService.RegisterHandler<BaseQuery, UserDto>(query, Assembly.GetExecutingAssembly(), typeof(UserDto));

            if (_handlers.TryGetValue(query.GetType(), out Type? handlerType))
            {
                try
                {
                    var handler = (IQueryHandler)_serviceProvider.GetRequiredService(handlerType);
                    return (UserDto)await handler.HandleAsync(query);
                }
                catch (Exception e)
                {
                    _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                    throw new QueryApplicationException("Auth dispatcher failed", e);
                }
            }
            _logger.LogError($"Could not find handler for query. Type: {query.GetType().Name}");
            return new UserDto();
        }
    }
}