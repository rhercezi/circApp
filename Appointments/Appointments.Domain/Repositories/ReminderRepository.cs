using Appointments.Domain.Configs;
using Core.DAOs;
using Core.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Appointments.Domain.Repositories
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
                _collection.Indexes.CreateOneAsync(new CreateIndexModel<ReminderModel>(Builders<ReminderModel>.IndexKeys.Ascending(c => c.TargetId),
                                                                                     new CreateIndexOptions { Name = "TargetId" }));
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
    }
}