using System.Collections.Concurrent;
using System.Reflection;
using Core.Messages;

namespace Core.MessageHandling
{
    public class HandlerService : IHandlerService
    {
        private readonly ConcurrentDictionary<Type, Type> _handlers = new();
        private readonly IServiceProvider _serviceProvider;

        public HandlerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }   
        
        public Dictionary<Type, Type> RegisterHandler<T>(BaseCommand message, Assembly assembly)
        {
            if (!_handlers.ContainsKey(message.GetType()))
            {
                Type genericType = typeof(ICommandHandler<>).MakeGenericType(message.GetType());

                var types = assembly.GetTypes().Where(
                    t => t.IsClass && t.GetInterfaces().Contains(genericType)
                ).ToList();

                if (types.Any())
                {
                    _handlers.TryAdd(message.GetType(), types.First());
                }
                else
                {
                    throw new ArgumentNullException($"No handler found for the command of type: {message.GetType().FullName}");
                }
            }

            return new Dictionary<Type, Type>(_handlers);
        }

        public Dictionary<Type, Type> RegisterHandler<T>(BaseEvent message, Assembly assembly)
        {
            if (!_handlers.ContainsKey(message.GetType()))
            {
                Type genericType = typeof(IEventHandler<>).MakeGenericType(message.GetType());

                var types = assembly.GetTypes().Where(
                    t => t.IsClass && t.GetInterfaces().Contains(genericType)
                ).ToList();

                if (types.Any())
                {
                    _handlers.TryAdd(message.GetType(), types.First());
                }
                else
                {
                    throw new ArgumentNullException($"No handler found for the event of type: {message.GetType().FullName}");
                }
            }

            return new Dictionary<Type, Type>(_handlers);
        }

        public Dictionary<Type, Type> RegisterHandler<T, R>(BaseQuery message, Assembly assembly, Type type)
        {
            if (!_handlers.ContainsKey(message.GetType()))
            {
                Type genericType = typeof(IQueryHandler<,>).MakeGenericType(message.GetType(), type);

                var types = assembly.GetTypes().Where(
                    t => t.IsClass && t.GetInterfaces().Contains(genericType)
                ).ToList();

                if (types.Any())
                {
                    _handlers.TryAdd(message.GetType(), types.First());
                }
                else
                {
                    throw new ArgumentNullException($"No handler found for the query of type: {message.GetType().FullName}");
                }
            }

            return new Dictionary<Type, Type>(_handlers);
        }

        public Dictionary<Type, Type> RegisterHandler<T>(BaseMessage message, Assembly assembly)
        {
            if (message != null && !_handlers.ContainsKey(message.GetType()))
            {
                Type genericType = typeof(IMessageHandler<>).MakeGenericType(message.GetType());

                var handlerType = assembly.GetTypes()
                    .FirstOrDefault(t => t.IsClass && t.GetInterfaces().Contains(genericType));

                if (handlerType == null)
                {
                    throw new ArgumentNullException($"No handler found for the message of type: {message.GetType().FullName}");
                }

                _handlers.TryAdd(message.GetType(), handlerType);
            }

            return new Dictionary<Type, Type>(_handlers);
        }
    }
}