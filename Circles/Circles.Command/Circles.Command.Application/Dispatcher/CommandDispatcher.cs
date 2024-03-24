using System.Reflection;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Circles.Command.Application.Dispatcher
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private Dictionary<Type, Type> _handlers = new();
        private readonly IHandlerService _handlerService;
        private readonly ILogger<CommandDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;

        public CommandDispatcher(IServiceProvider serviceProvider, ILogger<CommandDispatcher> logger, IHandlerService handlerService)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _handlerService = handlerService;
        }

        public async Task<(int code, string message)> DispatchAsync(BaseCommand command)
        {
            _handlers = _handlerService.RegisterHandler<BaseCommand>(command, Assembly.GetExecutingAssembly());

            if (_handlers.TryGetValue(command.GetType(), out var handlerType))
            {
                try
                {
                    var handler = (ICommandHandler)_serviceProvider.GetRequiredService(handlerType);
                    await handler.HandleAsync(command);
                    return (200, "Ok");
                }
                catch (Exception e)
                {
                    _logger.LogError($"{e.Message}\n{e.StackTrace}");
                    throw;
                }
            }
            else
            {
                _logger.LogError($"Handler not found for command of type '{command.GetType().Name}'");
                return (500, "Something went wrong, please contact support using support page.");
            }
        }
    }
}