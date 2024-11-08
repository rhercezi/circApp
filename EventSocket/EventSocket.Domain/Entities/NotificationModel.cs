using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EventSocket.Domain.Entities
{
    public class NotificationModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }
        public bool IsRead { get; set; }
        public NotificationBodyModel Body { get; set; }
    }
}