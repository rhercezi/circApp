using MongoDB.Bson.Serialization.Attributes;

namespace Core.DTOs
{
    [BsonIgnoreExtraElements]
    public class CircleDto : BaseDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Color { get; set; }
        public List<AppUserDto>? Users { get; set; }
    }
}