using Circles.Domain.Config;
using Circles.Domain.Entities;
using Circles.Domain.Exceptions;
using Core.DTOs;
using Core.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Circles.Domain.Repositories
{
    public class CirclesRepository : BaseRepository<CircleModel, CirclesRepository, MongoDbCirclesConfig>
    {
        public CirclesRepository(ILogger<CirclesRepository> logger, IOptions<MongoDbCirclesConfig> config) : base(logger, config)
        {
        }

        protected override void CreateIndexesIfNotExisting()
        {
            if (!_collection.Indexes.List().ToList().Where(i => i.Contains("name") && i["name"].AsString == "CircleId").Any())
            {
                _collection.Indexes.CreateOneAsync(new CreateIndexModel<CircleModel>(Builders<CircleModel>.IndexKeys.Ascending(c => c.CircleId),
                                                                                     new CreateIndexOptions { Name = "CircleId" }));
            }
        }

        public async Task<CircleModel> GetByIdAsync(Guid id)
        {
            var filter = Builders<CircleModel>.Filter.Eq(c => c.CircleId, id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<CircleModel>> GetByIdsAsync(List<Guid> ids)
        {
            var filter = Builders<CircleModel>.Filter.In(c => c.CircleId, ids);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<CircleModel> CheckExistsForUser(string name, Guid creator)
        {
            var filter = Builders<CircleModel>.Filter.And(
                Builders<CircleModel>.Filter.Eq(c => c.Name, name),
                Builders<CircleModel>.Filter.Eq(c => c.CreatorId, creator)
            );

            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<CircleDto> GetUsersInCircle(Guid circleId)
        {
            var filter = Builders<CircleModel>.Filter.Eq(c => c.CircleId, circleId);
            var aggregation = await _collection.Aggregate().Match(filter)
                                         .Lookup("circle.user.map", "_id", "CircleId", "Map")
                                         .Lookup("users", "Map.UserId", "_id", "Users")
                                         .As<CircleDto>()
                                         .FirstOrDefaultAsync();

            return aggregation;
        }

        public async Task SaveAsync(CircleModel circle)
        {
            try
            {
                await _collection.InsertOneAsync(circle).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                throw;
            }
        }

        public async Task UpdateCirce(CircleModel circle)
        {
            var filter = Builders<CircleModel>.Filter.Eq(c => c.CircleId, circle.CircleId);
            var update = Builders<CircleModel>.Update.Set(c => c.Name, circle.Name)
                                                     .Set(c => c.Color, circle.Color);
            var result = await _collection.UpdateOneAsync(filter, update);

            if (!result.IsAcknowledged && result.ModifiedCount == 0)
            {
                _logger.LogError($"Circle update failed for object: {circle}");
                throw new CirclesDbException($"Circle update failed for object: {circle}");
            }

        }

        public async Task DeleteCircle(Guid circleId)
        {
            var filter = Builders<CircleModel>.Filter.Eq(c => c.CircleId, circleId);
            var result = await _collection.DeleteOneAsync(filter);

            if (!result.IsAcknowledged && result.DeletedCount == 0)
            {
                _logger.LogError($"Delete circle failed for id: {circleId}");
                throw new CirclesDbException($"Delete circle failed for id: {circleId}");
            }
        }
    }
}