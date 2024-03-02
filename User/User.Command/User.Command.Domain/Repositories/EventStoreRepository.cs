using Core.Configs;
using Core.Messages;
using Core.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using User.Command.Domain.Events;
using User.Common.DAOs;

namespace User.Command.Application.Repositories
{
    public class EventStoreRepository : IEventStoreRepository<UserEventModel>
    {
        private readonly IMongoCollection<UserEventModel> _userCollection;
        private readonly MongoClient _client;
        private readonly EventProducer _eventProducer;

        public EventStoreRepository(IOptions<MongoDbConfig> config, EventProducer eventProducer)
        {
            this.SetMappers();
            _client = new MongoClient(config.Value.ConnectionString);
            var database = _client.GetDatabase(config.Value.DatabaseName);
            _userCollection = database.GetCollection<UserEventModel>(config.Value.CollectionName);
            _eventProducer = eventProducer;
        }

        public async Task<List<UserEventModel>> FindByAgregateId(Guid id)
        {
                return await _userCollection.Find(i => i.AggregateId == id).ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<UserEventModel>> FindByUsername(string username)
        {
            return await _userCollection.Find(i => i.UserName == username).ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<UserEventModel>> FindByEmail(string email)
        {
            return await _userCollection.Find(i => i.Email == email).ToListAsync().ConfigureAwait(false);
        }

        public async Task SaveAsync(UserEventModel model)
        {
            using var session = await _client.StartSessionAsync();
            await _userCollection.InsertOneAsync(model).ConfigureAwait(false);
            await _eventProducer.ProduceAsync(model.Event);
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
                t => {if (!BsonClassMap.IsClassMapRegistered(t)) BsonClassMap.LookupClassMap(t);}
            );
        }
    }
}