using Core.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using User.Common.DAOs;

namespace User.Command.Domain.Repositories
{
    public class IdLinkRepository : IMongoRepository<IdLinkModel>
    {
        private readonly IMongoCollection<IdLinkModel> _idLinkCollection;
        private readonly MongoClient _client;
        private readonly IConfiguration _configuration;
        private readonly ILogger<IdLinkRepository> _logger;

        public IdLinkRepository(IConfiguration configuration, ILogger<IdLinkRepository> logger)
        {
            _logger = logger;
            _configuration = configuration;
            _client = new MongoClient(_configuration["MongoDbConfigIDLink:ConnectionString"]);
            var database = _client.GetDatabase(_configuration["MongoDbConfigIDLink:DatabaseName"]);
            _idLinkCollection = database.GetCollection<IdLinkModel>(_configuration["MongoDbConfigIDLink:CollectionName"]);
        }

        public async Task<List<IdLinkModel>> GetByIdAsync(string idLink)
        {
            return await _idLinkCollection.Find(i => i.LinkId == idLink).ToListAsync().ConfigureAwait(false);
        }

        public async Task SaveAsync(IdLinkModel idLinkModel)
        {
            using var session = await _client.StartSessionAsync();
            try
            {
                await _idLinkCollection.InsertOneAsync(idLinkModel).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error saving IdLinkModel");
            }
        }

        public async Task DeleteAsync(string idLink)
        {
            var filter = Builders<IdLinkModel>.Filter.Eq(m => m.LinkId, idLink);
            await _idLinkCollection.DeleteOneAsync(filter);
        }
    }
}