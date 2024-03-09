using MongoDB.Bson.Serialization.Attributes;

namespace Core.DTOs
{
    [BsonIgnoreExtraElements]
    public class AppUserDto : BaseDto
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? FamilyName { get; set; }
        public string? Email { get; set; }
        public List<CircleDto>? Circles { get; set; }
    }
}