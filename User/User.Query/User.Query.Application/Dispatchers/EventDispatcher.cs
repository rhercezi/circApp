using System.Reflection;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace User.Query.Application.Dispatchers
{
    public class EventDispatcher : IEventDispatcher
    {
        private Dictionary<Type, Type> _handlers = new();
        private readonly ILogger<EventDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHandlerService _handlerService;

        public EventDispatcher(IHandlerService handlerService, IServiceProvider serviceProvider, ILogger<EventDispatcher> logger)
        {
            _handlerService = handlerService;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task<bool> DispatchAsync(BaseEvent xEvent)
        {
            _handlers = _handlerService.RegisterHandler<BaseEvent>(xEvent, Assembly.GetExecutingAssembly());

            if (_handlers.TryGetValue(xEvent.GetType(), out var handlerType))
            {
                try
                {
                    using var scope = _serviceProvider.CreateAsyncScope();
                    var handler = (IEventHandler)scope.ServiceProvider.GetRequiredService(handlerType);
                    await handler.HandleAsync(xEvent);
                }
                catch (Exception e)
                {
                    _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                    return false;
                }
                return true;
            }
            else return false;
        }
    }
}