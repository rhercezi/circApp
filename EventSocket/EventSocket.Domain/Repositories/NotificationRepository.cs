using Core.Repositories;
using EventSocket.Domain.Config;
using EventSocket.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EventSocket.Domain.Repositories
{
    public class NotificationRepository : BaseRepository<NotificationModel, NotificationRepository, MongoDbNotificationConfig>
    {
        public NotificationRepository(ILogger<NotificationRepository> logger, IOptions<MongoDbNotificationConfig> config) : base(logger, config)
        {
        }

        protected override void CreateIndexesIfNotExisting()
        {
            if (!_collection.Indexes.List().ToList().Where(i => i.Contains("name") && i["name"].AsString == "Id").Any())
            {
                _collection.Indexes.CreateOneAsync(new CreateIndexModel<NotificationModel>(Builders<NotificationModel>.IndexKeys.Ascending(t => t.Id),
                                                                                     new CreateIndexOptions { Name = "Id" }));
            }

            if (!_collection.Indexes.List().ToList().Where(i => i.Contains("name") && i["name"].AsString == "UserId").Any())
            {
                _collection.Indexes.CreateOneAsync(new CreateIndexModel<NotificationModel>(Builders<NotificationModel>.IndexKeys.Ascending(t => t.UserId),
                                                                                     new CreateIndexOptions { Name = "UserId" }));
            }
        }

        public async Task AddNotificationAsync(NotificationModel notification)
        {
            await _collection.InsertOneAsync(notification);
        }

        public async Task<List<NotificationModel>> GetNotificationsByUserIdAsync(Guid id)
        {
            return await _collection.Find(n => n.UserId == id).ToListAsync();
        }

        public async Task<DeleteResult> DeleteNotificationAsync(Guid notificationId)
        {
            return await _collection.DeleteOneAsync(n => n.Id == notificationId);
        }
    }
}