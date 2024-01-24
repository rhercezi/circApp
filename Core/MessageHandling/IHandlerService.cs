using System.Reflection;
using Core.Messages;

namespace Core.MessageHandling
{
    public interface IHandlerService
    {
        Dictionary<Type, Type> RegisterHandler<T>(BaseCommand message, Assembly assembly);
        Dictionary<Type, Type> RegisterHandler<T>(BaseEvent message, Assembly assembly);
        Dictionary<Type, Type> RegisterHandler<T,R>(BaseQuery message, Assembly assembly, Type type);
    }
}