using Core.Aggregate;
using Core.Messages;

namespace Core.MessageHandling
{
    public interface IAggregateAction<T,G> where T : BaseEvent where G : AbstractAggregate
    {
        Task ExecuteAsync(T xEvent, G instance, bool isReplay);
    }
}