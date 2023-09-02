using System.Reflection;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;

namespace User.Command.Application.Dispatcher
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private Dictionary<Type, Type> _handlers = new();
        private readonly ILogger<CommandDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHandlerService _handlerService;

        public CommandDispatcher(ILogger<CommandDispatcher> logger, IServiceProvider serviceProvider, IHandlerService handlerService)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _handlerService = handlerService;
        }

        public async Task DispatchAsync(BaseCommand command)
        {
            _handlers = _handlerService.RegisterHandler(command);


            if (_handlers.TryGetValue(command.GetType(), out Type? handlerType))
            {
                var handler = Activator.CreateInstance(handlerType, new object[] {_serviceProvider});
                if (handler is not null)
                {
                    await (Task)handlerType.GetMethod("HandleAsync").Invoke(handler, new object[] { command });
                }
                else
                {
                    throw new TargetException($"Faild to invoke handler of type: {handlerType.FullName}");
                }
            }

        }
    }
}