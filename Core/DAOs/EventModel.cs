using Core.Messages;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.DAOs
{
    public class EventModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public Guid AggregateId { get; set; }
        public string? AggregateType { get; set; }
        public int Version { get; set; }
        public string? EventType { get; set; }
        public BaseEvent? Event { get; set; }
    }
}