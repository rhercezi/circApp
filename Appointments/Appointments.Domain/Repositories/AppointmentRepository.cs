using Appointments.Domain.Configs;
using Appointments.Domain.Entities;
using Core.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Appointments.Domain.Repositories
{
    public class AppointmentRepository : BaseRepository<AppointmentModel, AppointmentRepository, MongoDbAppointmentsConfig>
    {
        public AppointmentRepository(ILogger<AppointmentRepository> logger, IOptions<MongoDbAppointmentsConfig> config) : base(logger, config)
        {
        }

        protected override void CreateIndexesIfNotExisting()
        {
            if (!_collection.Indexes.List().ToList().Where(i => i.Contains("name") && i["name"].AsString == "Id").Any())
            {
                _collection.Indexes.CreateOneAsync(new CreateIndexModel<AppointmentModel>(Builders<AppointmentModel>.IndexKeys.Ascending(c => c.Id),
                                                                                     new CreateIndexOptions { Name = "Id" }));
            }
        }

        public async Task<List<AppointmentModel>> GetAppointments(List<Guid> appointmentIds)
        {
            try
            {
                var filter = Builders<AppointmentModel>.Filter.In(a => a.Id, appointmentIds);
                return await _collection.Find(filter).ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return new List<AppointmentModel>();
            }
        }
        
        public async Task<AppointmentModel> GetAppointmentById(Guid appointmentId)
        {
            try
            {
                var filter = Builders<AppointmentModel>.Filter.Eq(a => a.Id, appointmentId);
                return await _collection.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                throw;
            }
        }

        public async Task SaveAsync(AppointmentModel appointment)
        {
            try
            {
                await _collection.InsertOneAsync(appointment);
            }
            catch (Exception e)
            {
                _logger.LogError("An error occurred while inserting the appointment\n {Message}\n{StackTrace}", e.Message, e.StackTrace);
                throw;
            }
        }

        public async Task<UpdateResult> UpdateAppointment(AppointmentModel appointment)
        {
            try
            {
                var filter = Builders<AppointmentModel>.Filter.And(
                    Builders<AppointmentModel>.Filter.Eq(a => a.Id, appointment.Id),
                    Builders<AppointmentModel>.Filter.Eq(a => a.CreatorId, appointment.CreatorId)
                );
                var update = Builders<AppointmentModel>.Update.Set(a => a.Date, appointment.Date)
                                                              .Set(a => a.DetailsInCircles, appointment.DetailsInCircles);
                return await _collection.UpdateOneAsync(filter, update);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                throw;
            }            
        }
        public async Task<DeleteResult> DeleteAppointment(Guid appointmentId, Guid userId)
        {
            try
            {
                var filter = Builders<AppointmentModel>.Filter.And(
                    Builders<AppointmentModel>.Filter.Eq(a => a.Id, appointmentId),
                    Builders<AppointmentModel>.Filter.Eq(a => a.CreatorId, userId)
                );
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