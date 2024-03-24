using System.Reflection;
using Appointments.Command.Application.Exceptions;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Appointments.Command.Application.Dispatchers
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private Dictionary<Type, Type> _handlers = new();
        private readonly IHandlerService _handlerService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CommandDispatcher> _logger;

        public CommandDispatcher(IHandlerService handlerService, IServiceProvider serviceProvider, ILogger<CommandDispatcher> logger)
        {
            _handlerService = handlerService;
            _serviceProvider = serviceProvider;
            _logger = logger;
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
                catch (AppointmentsApplicationException e)
                {
                    return (400, e.Message);
                }
                catch (Exception e)
                {
                    _logger.LogError($"{e.Message}\n{e.StackTrace}");
                    return (500, "Something went wrong, please contact support via support page.");
                }
            }
            return (500, "Something went wrong, please contact support via support page.");
        }
    }
}