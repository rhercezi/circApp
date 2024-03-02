using System.Linq.Expressions;
using Circles.Domain.Config;
using Circles.Domain.Entities;
using Core.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Circles.Domain.Repositories
{
    public class JoinRequestRepository : BaseRepository<JoinRequestModel, JoinRequestRepository, MongoDbRequestsConfig>
    {
        public JoinRequestRepository(ILogger<JoinRequestRepository> logger, IOptions<MongoDbRequestsConfig> config) : base(logger, config)
        {
        }

        protected override void CreateIndexesIfNotExisting()
        {
            if (!_collection.Indexes.List().ToList().Where(i => i.Contains("name") && i["name"].AsString == "UserId").Any())
            {
                _collection.Indexes.CreateOneAsync(new CreateIndexModel<JoinRequestModel>(Builders<JoinRequestModel>.IndexKeys.Ascending(u => u.UserId),
                                                                                     new CreateIndexOptions { Name = "UserId" }));
            }

            if (!_collection.Indexes.List().ToList().Where(i => i.Contains("name") && i["name"].AsString == "CircleId").Any())
            {
                _collection.Indexes.CreateOneAsync(new CreateIndexModel<JoinRequestModel>(Builders<JoinRequestModel>.IndexKeys.Ascending(u => u.UserId),
                                                                                     new CreateIndexOptions { Name = "CircleId" }));
            }
        }

        public async Task SaveAsync(JoinRequestModel model)
        {
            await _collection.InsertOneAsync(model).ConfigureAwait(false);
        }

        public async Task DeleteByPredicate(Expression<Func<JoinRequestModel, bool>> predicate)
        {
            var filter = Builders<JoinRequestModel>.Filter.Where(predicate);
            await _collection.DeleteManyAsync(filter);
        }
    }
}