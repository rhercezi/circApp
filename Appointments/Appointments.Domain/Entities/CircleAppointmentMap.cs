using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Appointments.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class CircleAppointmentMap
    {
        [BsonId]
        public Guid Id { get; set; }
        [BsonRepresentation(BsonType.String)]
        public required Guid AppointmentId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public required Guid CircleId { get; set; }
        public required DateTime Date { get; set; }
        
    }
}