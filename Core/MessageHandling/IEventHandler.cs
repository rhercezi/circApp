using Core.Messages;

namespace Core.MessageHandling
{
    public interface IEventHandler<T> where T : BaseEvent
    {
        Task HandleAsync(T xEvent);
    }
}