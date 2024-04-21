using System.Reflection;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using User.Command.Application.Exceptions;
using User.Command.Domain.Exceptions;

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

        public async Task<(int code, string message)> DispatchAsync(BaseCommand command)
        {
            _handlers = _handlerService.RegisterHandler<BaseCommand>(command, Assembly.GetExecutingAssembly());

            if (_handlers.TryGetValue(command.GetType(), out Type? handlerType))
            {
                try
                {
                    var handler = (ICommandHandler)_serviceProvider.GetRequiredService(handlerType);
                    await handler.HandleAsync(command);
                }
                catch (UserValidationException e)
                {
                    _logger.LogWarning(e.Message);
                    return (422, e.Message);
                }
                catch (UserDomainException e)
                {
                    _logger.LogWarning(e.Message);
                    return (422, e.Message);
                }
                catch (Exception e)
                {
                    _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                    return (500, "Something went wrong, please try again later.");
                }
                return (200, "Ok");

            }

            _logger.LogError($"Faild to invoke handler of type: {handlerType.FullName}");
            return (400, "Something went wrong, please try again later.");
        }
    }
}