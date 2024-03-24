using Appointments.Domain.Configs;
using Appointments.Domain.Entities;
using Core.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Appointments.Domain.Repositories
{
    public class CAMapRepository : BaseRepository<CircleAppointmentMap, CAMapRepository, MongoDbCAMapConfig>
    {
        public CAMapRepository(ILogger<CAMapRepository> logger, IOptions<MongoDbCAMapConfig> config) : base(logger, config)
        {
        }

        protected override void CreateIndexesIfNotExisting()
        {
            if (!_collection.Indexes.List().ToList().Where(i => i.Contains("name") && i["name"].AsString == "CircleId").Any())
            {
                _collection.Indexes.CreateOneAsync(new CreateIndexModel<CircleAppointmentMap>(Builders<CircleAppointmentMap>.IndexKeys.Ascending(c => c.CircleId),
                                                                                     new CreateIndexOptions { Name = "CircleId" }));
            }
            if (!_collection.Indexes.List().ToList().Where(i => i.Contains("name") && i["name"].AsString == "AppointmentId").Any())
            {
                _collection.Indexes.CreateOneAsync(new CreateIndexModel<CircleAppointmentMap>(Builders<CircleAppointmentMap>.IndexKeys.Ascending(c => c.AppointmentId),
                                                                                     new CreateIndexOptions { Name = "AppointmentId" }));
            }
        }

        public async Task<List<Guid>> GetAppointmentsByCircles(List<Guid> circles)
        {
            var filter = Builders<CircleAppointmentMap>.Filter.In(ca => ca.CircleId, circles);
            return await _collection.Distinct(ac => ac.AppointmentId, filter).ToListAsync();
        }

        public async Task<List<Guid>> GetAppointmentsByCircleId(Guid circle)
        {
            var filter = Builders<CircleAppointmentMap>.Filter.Eq(ca => ca.CircleId, circle);
            return await _collection.Distinct(ac => ac.AppointmentId, filter).ToListAsync();
        }

        public async Task SaveAsync(CircleAppointmentMap mapping)
        {
            try
            {
                await _collection.InsertOneAsync(mapping);
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message}\n{e.StackTrace}");
                throw;
            }
        }
        public async Task SaveManyAsync(List<CircleAppointmentMap> mappings)
        {
            try
            {
                await _collection.InsertManyAsync(mappings);
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message}\n{e.StackTrace}");
                throw;
            }
        }
        public async Task<DeleteResult> DeleteByAppointmentIdAsync(Guid appointmentId)
        {
            try
            {
                var filter = Builders<CircleAppointmentMap>.Filter.Eq(c => c.AppointmentId, appointmentId);
                return await _collection.DeleteManyAsync(filter);
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message}\n{e.StackTrace}");
                throw;
            }
        }
    }
}