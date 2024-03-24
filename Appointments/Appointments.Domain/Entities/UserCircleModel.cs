using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Appointments.Domain.Entities
{
    public class UserCircleModel
    {
        public UserCircleModel()
        {
            Id = Guid.NewGuid().ToString();
        }

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public required Guid UserId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public required Guid CircleId { get; set; }
    }
}