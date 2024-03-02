namespace Core.Repositories
{
    public interface IMongoRepository<T>
    {
        Task SaveAsync(T model);
        Task<List<T>> GetByIdAsync(string id); 
        Task DeleteAsync(string id);
    }
}