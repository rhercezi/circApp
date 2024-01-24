using Core.Messages;

namespace Core.MessageHandling
{
    public interface ICommandDispatcher
    {
        Task<(int code, string message)> DispatchAsync(BaseCommand command);
    }
}