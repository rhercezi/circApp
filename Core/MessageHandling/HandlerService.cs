using System.Reflection;
using Core.Messages;

namespace Core.MessageHandling
{
    public class HandlerService : IHandlerService
    {
        private Dictionary<Type, Type> _handlers = new();
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
                    _handlers.Add(message.GetType(), types.First());
                }  
                else
                {
                    throw new ArgumentNullException($"No handler found for the command of type: {message.GetType().FullName}");
                }
            }

            return _handlers;
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
                    _handlers.Add(message.GetType(), types.First());
                }  
                else
                {
                    throw new ArgumentNullException($"No handler found for the event of type: {message.GetType().FullName}");
                }
            }

            return _handlers;
        }

        public Dictionary<Type, Type> RegisterHandler<T,R>(BaseQuery message, Assembly assembly, Type type)
        {
            if (!_handlers.ContainsKey(message.GetType()))
            {
                Type genericType = typeof(IQueryHandler<,>).MakeGenericType(message.GetType(), type);

                var types = assembly.GetTypes().Where(
                    t => t.IsClass && t.GetInterfaces().Contains(genericType)
                ).ToList();

                if (types.Any())
                {
                    _handlers.Add(message.GetType(), types.First());
                }  
                else
                {
                    throw new ArgumentNullException($"No handler found for the query of type: {message.GetType().FullName}");
                }
            }

            return _handlers;
        }
    }
}