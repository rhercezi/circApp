using Core.DAOs;

namespace Core.Repositories
{
    public interface IEventStoreRepository<T> where T : EventModel
    {
        Task SaveAsync(T model);
        Task<List<T>> FindByAgregateId(Guid id);
    }
}