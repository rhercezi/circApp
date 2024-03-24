using Core.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using User.Command.Domain.Config;
using User.Common.DAOs;

namespace User.Command.Domain.Repositories
{
    public class IdLinkRepository : BaseRepository<IdLinkModel, IdLinkRepository, MongoDbConfigIDLink>
    {
        public IdLinkRepository(ILogger<IdLinkRepository> logger, IOptions<MongoDbConfigIDLink> config) : base(logger, config)
        {
        }

        public async Task<List<IdLinkModel>> GetByIdAsync(string idLink)
        {
            return await _collection.Find(i => i.LinkId == idLink).ToListAsync().ConfigureAwait(false);
        }

        public async Task SaveAsync(IdLinkModel idLinkModel)
        {
            using var session = await _client.StartSessionAsync();
            try
            {
                await _collection.InsertOneAsync(idLinkModel).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error saving IdLinkModel");
            }
        }

        public async Task DeleteAsync(string idLink)
        {
            var filter = Builders<IdLinkModel>.Filter.Eq(m => m.LinkId, idLink);
            await _collection.DeleteOneAsync(filter);
        }

        protected override void CreateIndexesIfNotExisting()
        {
            throw new NotImplementedException();
        }
    }
}