using System.Reflection;
using Core.MessageHandling;
using Core.Messages;

namespace User.Command.Application.Handlers
{
    public class UserHandlerService : IHandlerService
    {
        private Dictionary<Type, Type> _handlers = new();
        public Dictionary<Type, Type> RegisterHandler<T>(T command) where T : BaseCommand
        {
             if (!_handlers.ContainsKey(command.GetType()))
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

            return _handlers;
        }
    }
}