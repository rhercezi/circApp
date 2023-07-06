using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Messages;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.DAOs
{
    public class EventModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public Guid AggregateId { get; set; }
        public required string AggregateType { get; set; }
        public int Version { get; set; }
        public required string EventType { get; set; }
        public required BaseEvent Event { get; set; }
    }
}