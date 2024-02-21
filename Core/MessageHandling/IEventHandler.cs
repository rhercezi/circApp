using Core.Messages;

namespace Core.MessageHandling
{
    public interface IEventHandler
    {
        Task HandleAsync(BaseEvent xEvent);
    }
    public interface IEventHandler<T> : IEventHandler where T : BaseEvent
    {
        Task HandleAsync(T xEvent);
    }
}