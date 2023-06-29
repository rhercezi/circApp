using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;

namespace User.Command.Application.Dispatcher
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private Dictionary<Type, Type> _handlers = new();
        private readonly ILogger<CommandDispatcher> _logger;

        public CommandDispatcher(ILogger<CommandDispatcher> logger)
        {
            _logger = logger;
        }

        public async Task DispatchAsync(BaseCommand command)
        {
            if (!_handlers.ContainsKey(command.GetType()))
            {
                RegisterHandler(command);
            }


            if (_handlers.TryGetValue(command.GetType(), out Type? handlerType))
            {
                var handler = Activator.CreateInstance(handlerType) as ICommandHandler<BaseCommand>;
                if (handler is not null)
                {
                    await handler.HandleAsync(command);
                }
                else
                {
                    throw new TargetException($"Faild to invoke handler of type: {handlerType.FullName}");
                }
            }

        }

        public void RegisterHandler<T>(T command) where T : BaseCommand
        {
            Type genericType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());

            var types = Assembly.GetExecutingAssembly().GetTypes().Where(
                t => t.IsClass && t.GetInterfaces().Contains(genericType)
            ).ToList();

            if (types.Any())
            {
                _handlers.Add(command.GetType(), types.First());
            }  
            else
            {
                throw new ArgumentNullException($"No handler found for the command of type: {command.GetType().FullName}");
            }
        }
    }
}