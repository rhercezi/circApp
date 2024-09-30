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
        public required string Title { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public AppointmentDetailsModel? Details { get; set; }
        public List<Guid>? DetailsInCircles { get; set; }
        public required List<Guid> Circles { get; set; }
    }
}