using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Configs;
using Core.DAOs;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using User.Command.Domain.Repositories;

namespace User.Command.Application.Repositories
{
    public class EventStoreRepository : IEventStoreRepository
    {
        private readonly IMongoCollection<EventModel> _eventStoreCollection;

        public EventStoreRepository(IOptions<MongoDbConfig> config)
        {
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
    }
}