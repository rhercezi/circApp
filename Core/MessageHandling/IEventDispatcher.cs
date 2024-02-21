using Core.Messages;

namespace Core.MessageHandling
{
    public interface IEventDispatcher
    {
        Task<bool> DispatchAsync(BaseEvent xEvent);
    }
}