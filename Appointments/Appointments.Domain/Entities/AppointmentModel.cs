using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Appointments.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class AppointmentModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public required Guid Id { get; set; }
        [BsonRepresentation(BsonType.String)]
        public required Guid CreatorId { get; set; }
        public required DateTime Date { get; set; }
        public AppointmentDetailsModel? Details { get; set; }
        public List<Guid>? DetailsInCircles { get; set; }
    }
}