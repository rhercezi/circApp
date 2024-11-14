using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.DAOs
{
    public class ReminderModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public List<Guid>? InCircles { get; set; }
        public ReminderTargetType TargetType { get; set; }
        public Guid TargetId { get; set; }
        public bool IsSeen { get; set; }
        public required DateTime Time { get; set; }
        public string? Message { get; set; }
        public bool JustForUser { get; set; }
    }
}