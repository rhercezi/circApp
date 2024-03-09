using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Appointments.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class AppointmentModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string? Note { get; set; }
        public Address? Address { get; set; }
        public List<Reminder>? Reminders { get; set; }
    }
}