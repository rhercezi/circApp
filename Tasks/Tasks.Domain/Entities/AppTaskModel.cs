using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tasks.Domain.Entities
{
    public class AppTaskModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid OwnerId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid? ParentTaskId { get; set; }
        public List<TaskUserModel>? UserModels { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid? CircleId { get; set; }

    }
}   