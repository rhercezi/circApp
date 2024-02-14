using System.Reflection;
using Core.MessageHandling;
using Core.Messages;
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
                var handler = Activator.CreateInstance(handlerType, new object[] {_serviceProvider});
                if (handler is not null)
                {
                    try
                    {
                        var task = (Task)handlerType.GetMethod("HandleAsync").Invoke(handler, new object[] { command });
                        await task.ConfigureAwait(false);
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
                        _logger.LogError(e.StackTrace, e.Message);
                        return (500, "Something went wrong, please try again later.");
                    }
                    return(200, "Ok");
                }
            }

            _logger.LogError($"Faild to invoke handler of type: {handlerType.FullName}");
            return (400, "Something went wrong, please try again later.");
        }
    }
}