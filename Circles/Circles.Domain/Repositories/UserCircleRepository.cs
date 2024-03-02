using System.Linq.Expressions;
using Circles.Domain.Config;
using Circles.Domain.Entities;
using Core.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Circles.Domain.Repositories
{
    public class UserCircleRepository : BaseRepository<UserCircleModel, UserCircleRepository, MongoDbCircleUserMapConfig>
    {
        public UserCircleRepository(ILogger<UserCircleRepository> logger, IOptions<MongoDbCircleUserMapConfig> config) : base(logger, config)
        {
        }

        protected override void CreateIndexesIfNotExisting()
        {
            if (!_collection.Indexes.List().ToList().Where(i => i.Contains("name") && i["name"].AsString == "UserId").Any())
            {
                _collection.Indexes.CreateOneAsync(new CreateIndexModel<UserCircleModel>(Builders<UserCircleModel>.IndexKeys.Ascending(u => u.UserId),
                                                                                     new CreateIndexOptions { Name = "UserId" }));
            }

            if (!_collection.Indexes.List().ToList().Where(i => i.Contains("name") && i["name"].AsString == "CircleId").Any())
            {
                _collection.Indexes.CreateOneAsync(new CreateIndexModel<UserCircleModel>(Builders<UserCircleModel>.IndexKeys.Ascending(u => u.UserId),
                                                                                     new CreateIndexOptions { Name = "CircleId" }));
            }
        }

        public async Task SaveAsync(UserCircleModel model)
        {
            await _collection.InsertOneAsync(model).ConfigureAwait(false);
        }

        public async Task DeleteByPredicate(Expression<Func<UserCircleModel, bool>> predicate)
        {
            var filter = Builders<UserCircleModel>.Filter.Where(predicate);
            await _collection.DeleteManyAsync(filter);
        }
    }
}