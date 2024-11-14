using Core.DAOs;
using Core.Repositories;
using EventSocket.Domain.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EventSocket.Domain.Repositories
{
    public class ReminderRepository : BaseRepository<ReminderModel, ReminderRepository, MongoDbReminderConfig>
    {
        public ReminderRepository(ILogger<ReminderRepository> logger, IOptions<MongoDbReminderConfig> config) : base(logger, config)
        {
        }

        protected override void CreateIndexesIfNotExisting()
        {
            if (!_collection.Indexes.List().ToList().Where(i => i.Contains("name") && i["name"].AsString == "TargetId").Any())
            {
                _collection.Indexes.CreateOneAsync(new CreateIndexModel<ReminderModel>(Builders<ReminderModel>.IndexKeys.Ascending(c => c.Id),
                                                                                     new CreateIndexOptions { Name = "TargetId" }));
            }
            if (!_collection.Indexes.List().ToList().Where(i => i.Contains("name") && i["name"].AsString == "Id").Any())
            {
                _collection.Indexes.CreateOneAsync(new CreateIndexModel<ReminderModel>(Builders<ReminderModel>.IndexKeys.Ascending(c => c.Id),
                                                                                     new CreateIndexOptions { Name = "Id" }));
            }
        }

        public async Task SaveAsync(ReminderModel reminder)
        {
            try
            {
                await _collection.InsertOneAsync(reminder);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
            }
        }

        public async Task DeleteAsync(Guid appointmentId)
        {
            try
            {
                var filter = Builders<ReminderModel>.Filter.Eq(r => r.TargetId, appointmentId);
                await _collection.DeleteOneAsync(filter);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
            }
        }

        public async Task<List<ReminderModel>?> FindManyAsync(Guid id)
        {
            var filter = Builders<ReminderModel>.Filter.Eq(r => r.TargetId, id);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<List<ReminderModel>> FindRemindersToSendAsync(List<Guid> circles, Guid userId, DateTime timeTo)
        {
            var JustForUser = Builders<ReminderModel>.Filter.Eq(r => r.JustForUser, true);
            var uId = Builders<ReminderModel>.Filter.Eq(r => r.UserId, userId);
            var filter1 = Builders<ReminderModel>.Filter.And(JustForUser, uId);

            var filter2 = Builders<ReminderModel>.Filter.AnyIn(r => r.InCircles, circles);
            var userOrCircles= Builders<ReminderModel>.Filter.Or(filter1, filter2);

            var time = Builders<ReminderModel>.Filter.Lte(r => r.Time, timeTo);
            var seen = Builders<ReminderModel>.Filter.Eq(r => r.IsSeen, false);

            var filter = Builders<ReminderModel>.Filter.And(userOrCircles, time, seen);

            return await _collection.Find(filter).ToListAsync();
        }

        public async Task MarkAsSeenAsync(Guid reminderId)
        {
            var filter = Builders<ReminderModel>.Filter.Eq("Id", reminderId);
            var update = Builders<ReminderModel>.Update.Set(r => r.IsSeen, true);
            await _collection.UpdateOneAsync(filter, update);
        }
    }
}