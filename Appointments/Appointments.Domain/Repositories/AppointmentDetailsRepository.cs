using Appointments.Domain.Configs;
using Appointments.Domain.Entities;
using Core.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Appointments.Domain.Repositories
{
    public class AppointmentDetailsRepository : BaseRepository<AppointmentDetailsModel, AppointmentDetailsRepository, MongoDbAppointmentDetailsConfig>
    {
        public AppointmentDetailsRepository(ILogger<AppointmentDetailsRepository> logger, IOptions<MongoDbAppointmentDetailsConfig> config) : base(logger, config)
        {
        }

        protected override void CreateIndexesIfNotExisting()
        {
            if (!_collection.Indexes.List().ToList().Where(i => i.Contains("name") && i["name"].AsString == "AppointmentId").Any())
            {
                _collection.Indexes.CreateOneAsync(new CreateIndexModel<AppointmentDetailsModel>(Builders<AppointmentDetailsModel>.IndexKeys.Ascending(c => c.AppointmentId),
                                                                                     new CreateIndexOptions { Name = "AppointmentId" }));
            }
        }

        public async Task<AppointmentDetailsModel> FindAsync(Guid appointmentId)
        {
            var filter = Builders<AppointmentDetailsModel>.Filter.Eq(ad => ad.AppointmentId, appointmentId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task SaveAsync(AppointmentDetailsModel detailsModel)
        {
            try
            {
                await _collection.InsertOneAsync(detailsModel);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                throw;
            }
        }
        public async Task<ReplaceOneResult> UpdateAsync(AppointmentDetailsModel detailsModel)
        {
            try
            {
                var filter = Builders<AppointmentDetailsModel>.Filter.Eq(c => c.AppointmentId, detailsModel.AppointmentId);
                return await _collection.ReplaceOneAsync(filter, detailsModel);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                throw;
            }
        }

        public async Task<DeleteResult> DeleteAsync(Guid appointmentId)
        {
            try
            {
                var filter = Builders<AppointmentDetailsModel>.Filter.Eq(c => c.AppointmentId, appointmentId);
                return await _collection.DeleteOneAsync(filter);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                throw;
            }
        }
    }
}