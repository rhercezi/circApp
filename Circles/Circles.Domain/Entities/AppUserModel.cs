using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Circles.Domain.Entities
{
    public class AppUserModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }
        public required string UserName { get; set; }
        public required string FirstName { get; set; }
        public required string FamilyName { get; set; }
        public required string Email { get; set; }
    }
}