using Core.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Tasks.Domain.Config;
using Tasks.Domain.Entities;

namespace Tasks.Domain.Repositories
{
    public class AppTaskRepository : BaseRepository<AppTaskModel, AppTaskRepository, MongoDbTaskConfig>
    {
        public AppTaskRepository(ILogger<AppTaskRepository> logger, IOptions<MongoDbTaskConfig> config) : base(logger, config)
        {
        }

        protected override void CreateIndexesIfNotExisting()
        {
            if (!_collection.Indexes.List().ToList().Where(i => i.Contains("name") && i["name"].AsString == "Id").Any())
            {
                _collection.Indexes.CreateOneAsync(new CreateIndexModel<AppTaskModel>(Builders<AppTaskModel>.IndexKeys.Ascending(t => t.Id),
                                                                                     new CreateIndexOptions { Name = "Id" }));
            }

            if (!_collection.Indexes.List().ToList().Where(i => i.Contains("name") && i["name"].AsString == "CircleId").Any())
            {
                _collection.Indexes.CreateOneAsync(new CreateIndexModel<AppTaskModel>(Builders<AppTaskModel>.IndexKeys.Ascending(t => t.CircleId),
                                                                                     new CreateIndexOptions { Name = "CircleId" }));
            }

            if (!_collection.Indexes.List().ToList().Where(i => i.Contains("name") && i["name"].AsString == "UserId").Any())
            {
                _collection.Indexes.CreateOneAsync(new CreateIndexModel<AppTaskModel>(Builders<AppTaskModel>.IndexKeys.Ascending("UserModels.Id"),
                                                                                     new CreateIndexOptions { Name = "UserId" }));
            }
        }

        public async Task<List<AppTaskModel>> GetTasksByUserId(Guid userId, bool includeCompleted)
        {
            var filter1 = Builders<AppTaskModel>.Filter.ElemMatch(t => t.UserModels, Builders<TaskUserModel>.Filter.Eq(u => u.Id, userId));
            var filter2 = Builders<AppTaskModel>.Filter.And(
                Builders<AppTaskModel>.Filter.Eq(t => t.OwnerId, userId),
                Builders<AppTaskModel>.Filter.Eq(t => t.CircleId, null)
            );
            var filter = Builders<AppTaskModel>.Filter.Or(filter1, filter2);
            if (!includeCompleted)
            {
                filter1 = Builders<AppTaskModel>.Filter.And(filter, Builders<AppTaskModel>.Filter.Eq(t => t.IsCompleted, false));
                return await _collection.Find(filter1).ToListAsync();
            }
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task SaveAsync(AppTaskModel task)
        {
            try
            {
                await _collection.InsertOneAsync(task).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                throw;
            }
        }

        public async Task<ReplaceOneResult> UpdateTask(AppTaskModel task)
        {
            try
            {
                return await _collection.ReplaceOneAsync(t => t.Id == task.Id, task).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                throw;
            }
        }

        public async Task<DeleteResult> DeleteTask(Guid taskId)
        {
            try
            {
                return await _collection.DeleteOneAsync(t => t.Id == taskId).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                throw;
            }
        }

        public async Task<List<AppTaskModel>> GetTasksByCircleId(Guid circleId, bool includeCompleted)
        {
            var filter = Builders<AppTaskModel>.Filter.Eq(t => t.CircleId, circleId);
            if (!includeCompleted)
            {
                filter = Builders<AppTaskModel>.Filter.And(filter, Builders<AppTaskModel>.Filter.Eq(t => t.IsCompleted, false));
                return await _collection.Find(filter).ToListAsync();
            }
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<List<AppTaskModel>> GetTasksByCircleIds(List<Guid> circleIds, bool includeCompleted)
        {
            var filter = Builders<AppTaskModel>.Filter.In(t => t.CircleId, circleIds.Select(id => (Guid?)id));
            if (!includeCompleted)
            {
                filter = Builders<AppTaskModel>.Filter.And(filter, Builders<AppTaskModel>.Filter.Eq(t => t.IsCompleted, false));
                return await _collection.Find(filter).ToListAsync();
            }
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<List<AppTaskModel>> GetTasksByParentTaskId(Guid parentTaskId)
        {
            var filter = Builders<AppTaskModel>.Filter.Eq(t => t.ParentTaskId, parentTaskId);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<AppTaskModel> GetTaskById(Guid id)
        {
            var filter = Builders<AppTaskModel>.Filter.Eq(t => t.Id, id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }
    }
}