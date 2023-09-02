using Core.Messages;

namespace Core.MessageHandling
{
    public interface ICommandDispatcher
    {
        Task DispatchAsync(BaseCommand command);
    }
}