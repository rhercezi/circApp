using System.Reflection;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Circles.Command.Application.Dispatcher
{
    public class EventDispatcher : IEventDispatcher
    {
        private Dictionary<Type, Type> _handlers = new();
        private readonly ILogger<EventDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHandlerService _handlerService;
        public EventDispatcher(ILogger<EventDispatcher> logger, IServiceProvider serviceProvider, IHandlerService handlerService)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _handlerService = handlerService;
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
                    _logger.LogError($"{e.Message}\n{e.StackTrace}");
                    return false;
                }
                return true;
            }
            else return false;
        }
    }
}