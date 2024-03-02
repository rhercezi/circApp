using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Circles.Domain.Entities
{
    public class JoinRequestModel
    {
        public JoinRequestModel()
        {
            Id = Guid.NewGuid().ToString();
        }

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid CircleId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid InviterId { get; set; }
    }
}