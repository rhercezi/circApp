using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Appointments.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class AppointmentDetailsModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public required Guid AppointmentId { get; set; }
        public string? Note { get; set; }
        public Address? Address { get; set; }
        public List<Reminder>? Reminders { get; set; }
    }
}