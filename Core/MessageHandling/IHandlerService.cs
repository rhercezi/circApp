using Core.Messages;

namespace Core.MessageHandling
{
    public interface IHandlerService
    {
        Dictionary<Type, Type> RegisterHandler<T>(T command) where T : BaseCommand;
    }
}