
using Core.Configs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Core.Repositories
{
    public abstract class BaseRepository<Tmodel, Trepository, Tconfig> where Tconfig : MongoDbConfig
    {
        protected readonly IMongoCollection<Tmodel> _collection;
        protected readonly MongoClient _client;
        protected readonly ILogger<Trepository> _logger;
        protected readonly IOptions<Tconfig> _config;
        public BaseRepository(ILogger<Trepository> logger, IOptions<Tconfig> config)
        {
            _logger = logger;
            _config = config;
            _client = new MongoClient(_config.Value.ConnectionString);
            _collection = _client.GetDatabase(config.Value.DatabaseName)
                                 .GetCollection<Tmodel>(_config.Value.CollectionName);

            CreateIndexesIfNotExisting();
        }

        protected abstract void CreateIndexesIfNotExisting();

        public Task<IClientSessionHandle> GetSession()
        {
            try
            {
                return _client.StartSessionAsync();
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message}\n{e.StackTrace}");
                throw;
            }
        }
    }
}