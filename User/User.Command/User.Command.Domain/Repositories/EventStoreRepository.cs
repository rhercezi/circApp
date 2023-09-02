using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Core.Configs;
using Core.DAOs;
using Core.Messages;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using User.Command.Domain.Repositories;
using User.Common.Events;

namespace User.Command.Application.Repositories
{
    public class EventStoreRepository : IEventStoreRepository
    {
        private readonly IMongoCollection<EventModel> _eventStoreCollection;

        public EventStoreRepository(IOptions<MongoDbConfig> config)
        {
            this.SetMappers();
            // BsonClassMap.RegisterClassMap<BaseEvent>(cm => 
            // {
            //     cm.AutoMap();
            //     cm.SetIsRootClass(true);
            // });
            // BsonClassMap.RegisterClassMap<UserCreatedEvent>();
            // BsonClassMap.RegisterClassMap<UserEditedEvent>();
            // BsonClassMap.RegisterClassMap<UserDeletedEvent>();
            var client = new MongoClient(config.Value.ConnectionString);
            var database = client.GetDatabase(config.Value.DatabaseName);
            _eventStoreCollection = database.GetCollection<EventModel>(config.Value.CollectionName);
        }

        public async Task<List<EventModel>> FindByAgregateId(Guid id)
        {
            return await _eventStoreCollection.Find(i => i.AggregateId == id).ToListAsync().ConfigureAwait(false);
        }

        public async Task SaveAsync(EventModel model)
        {
            await _eventStoreCollection.InsertOneAsync(model).ConfigureAwait(false);
        }

        private void SetMappers()
        {
            var events = AppDomain.CurrentDomain.GetAssemblies().SelectMany(
                a => a.GetTypes().Where(
                    t => t.IsClass && t.IsSubclassOf(typeof(BaseEvent))
                )
            ).ToList();
            
            events.ForEach(
                t => {if (!BsonClassMap.IsClassMapRegistered(t)) BsonClassMap.LookupClassMap(t);}
            );
        }
    }
}