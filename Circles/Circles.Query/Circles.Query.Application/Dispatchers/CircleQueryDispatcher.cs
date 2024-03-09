using System.Reflection;
using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Circles.Query.Application.Dispatchers
{
    public class CircleQueryDispatcher : IQueryDispatcher<CircleDto>
    {
        private Dictionary<Type, Type> _handlers = new();
        private readonly ILogger<CircleQueryDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHandlerService _handlerService;
        public CircleQueryDispatcher(ILogger<CircleQueryDispatcher> logger,
                                     IServiceProvider serviceProvider,
                                     IHandlerService handlerService)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _handlerService = handlerService;
        }

        public async Task<CircleDto> DispatchAsync(BaseQuery query)
        {
            _handlers = _handlerService.RegisterHandler<BaseQuery, CircleDto>(query, Assembly.GetExecutingAssembly(), typeof(CircleDto));

            if (_handlers.TryGetValue(query.GetType(), out Type? handlerType))
            {
                try
                {
                    var handler = (IQueryHandler)_serviceProvider.GetRequiredService(handlerType);
                    return (CircleDto)await handler.HandleAsync(query);
                }
                catch (Exception e)
                {
                    _logger.LogError($"{e.Message}\n{e.StackTrace}");
                    throw;
                }
            }
            _logger.LogError($"Could not find handler for query. Type: {query.GetType().Name}");
            return new CircleDto();
        }
    }
}