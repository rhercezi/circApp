using Circles.Domain.Config;
using Circles.Domain.Entities;
using Circles.Domain.Exceptions;
using Core.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Circles.Domain.Repositories
{
    public class UserRepository : BaseRepository<AppUserModel, UserRepository, MongoDbUsersConfig>
    {
        public UserRepository(ILogger<UserRepository> logger, IOptions<MongoDbUsersConfig> config) : base(logger, config)
        {
        }

        protected override void CreateIndexesIfNotExisting()
        {
            if (!_collection.Indexes.List().ToList().Where(i => i.Contains("name") && i["name"].AsString == "UserId").Any())
            {
                _collection.Indexes.CreateOneAsync(new CreateIndexModel<AppUserModel>(Builders<AppUserModel>.IndexKeys.Ascending(u => u.UserId),
                                                                                     new CreateIndexOptions { Name = "UserId" }));
            }
        }

        public async Task<List<AppUserModel>> GetByIdsAsync(List<Guid> ids)
        {
            var filter = Builders<AppUserModel>.Filter.In(c => c.UserId, ids);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task SaveAsync(AppUserModel user)
        {
            try
            {
                await _collection.InsertOneAsync(user).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e.StackTrace, e.Message);
                throw;
            }
        }

        public async Task UpdateUser(AppUserModel user)
        {
            var filter = Builders<AppUserModel>.Filter.Eq(c => c.UserId, user.UserId);
            var update = Builders<AppUserModel>.Update.Set(u => u.FirstName, user.FirstName)
                                                      .Set(u => u.FamilyName, user.FamilyName)
                                                      .Set(u => u.UserName, user.UserName);
            var result = await _collection.UpdateOneAsync(filter, update);

            if (!result.IsAcknowledged || result.ModifiedCount == 0)
            {
                _logger.LogError($"User update failed for object: {user}");
                throw new CirclesDbException($"User update failed for object: {user}");
            }

        }

        public async Task DeleteUser(Guid userId)
        {
            var filter = Builders<AppUserModel>.Filter.Eq(c => c.UserId, userId);
            var result = await _collection.DeleteOneAsync(filter);

            if (!result.IsAcknowledged || result.DeletedCount == 0)
            {
                _logger.LogError($"Delete user failed for id: {userId}");
                throw new CirclesDbException($"Delete user failed for id: {userId}");
            }
        }
    }
}