using Core.Aggregate;

namespace Core.MessageHandling
{
    public interface IEventHandler<T>
    {
        Task SaveAsync(AbstractAggregate aggregate);
        Task<T> GetAsync(Guid id);
    }
}