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

        public async Task<List<JoinRequestModel>> FindAsync(Expression<Func<JoinRequestModel, bool>> expression)
        {
            var filter = Builders<JoinRequestModel>.Filter.Where(expression);
            var cursor = await _collection.FindAsync(filter);
            return cursor.ToList();
        }

        public async Task SaveAsync(JoinRequestModel model)
        {
            await _collection.InsertOneAsync(model).ConfigureAwait(false);
        }

        public async Task DeleteAsync(Guid userId, Guid circleId)
        {
            var userFilter = Builders<JoinRequestModel>.Filter.Eq(jr => jr.UserId, userId);
            var circleFilter = Builders<JoinRequestModel>.Filter.Eq(jr => jr.CircleId, circleId);
            var filter = Builders<JoinRequestModel>.Filter.And(new [] {userFilter, circleFilter});
            await _collection.DeleteOneAsync(filter);
        }
    }
}