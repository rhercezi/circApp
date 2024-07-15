using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Circles.Domain.Entities
{
    public class CircleModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid CircleId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid CreatorId { get; set; }
        public required string Name { get; set; }
        public required string Color { get; set; }
    }
}