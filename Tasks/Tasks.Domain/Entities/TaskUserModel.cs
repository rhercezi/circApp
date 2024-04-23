using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tasks.Domain.Entities
{
    public class TaskUserModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public required string UserName { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}