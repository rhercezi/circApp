using Core.Configs;
using Core.Messages;
using Core.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using User.Command.Domain.Events;
using User.Common.DAOs;

namespace User.Command.Application.Repositories
{
    public class EventStoreRepository : BaseRepository<UserEventModel, EventStoreRepository, MongoDbConfig>
    {
        private readonly EventProducer _eventProducer;
        public EventStoreRepository(ILogger<EventStoreRepository> logger, IOptions<MongoDbConfig> config, EventProducer eventProducer) : base(logger, config)
        {
            _eventProducer = eventProducer;
            SetMappers();
        }

        public async Task<List<UserEventModel>> FindByAgregateId(Guid id)
        {
            return await _collection.Find(i => i.AggregateId == id).ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<UserEventModel>> FindByUsername(string username)
        {
            return await _collection.Find(i => i.UserName == username).ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<UserEventModel>> FindByEmail(string email)
        {
            return await _collection.Find(i => i.Email == email).ToListAsync().ConfigureAwait(false);
        }

        public async Task SaveAsync(UserEventModel model)
        {
            using var session = await _client.StartSessionAsync();
            try
            {
                session.StartTransaction();
                await _collection.InsertOneAsync(model).ConfigureAwait(false);
                await _eventProducer.ProduceAsync(model.Event);
                session.CommitTransaction();
            }
            catch (Exception e)
            {
                session.AbortTransaction();
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                throw;
            }
        }

        private void SetMappers()
        {
            var events = AppDomain.CurrentDomain.GetAssemblies().SelectMany(
                a => a.GetTypes().Where(
                    t => t.IsClass &&
                    t.IsSubclassOf(typeof(BaseEvent))
                )
            ).ToList();

            events.ForEach(
                t => { if (!BsonClassMap.IsClassMapRegistered(t)) BsonClassMap.LookupClassMap(t); }
            );
        }

        protected override void CreateIndexesIfNotExisting()
        {
            if (!_collection.Indexes.List().ToList().Where(i => i.Contains("name") && i["name"].AsString == "AppointmentId").Any())
            {
                _collection.Indexes.CreateOneAsync(new CreateIndexModel<UserEventModel>(Builders<UserEventModel>.IndexKeys.Ascending(u => u.Id),
                                                                                     new CreateIndexOptions { Name = "Id" }));
            }
        }
    }
}