using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Core.DTOs
{
    [BsonIgnoreExtraElements]
    public class AppUserDto : BaseDto
    {
        public AppUserDto()
        {
            Circles = new List<CircleDto>();
        }
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("userName")]
        public string? UserName { get; set; }
        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }
        [JsonPropertyName("familyName")]
        public string? FamilyName { get; set; }
        [JsonPropertyName("email")]
        public string? Email { get; set; }
        [JsonPropertyName("circles")]
        public List<CircleDto>? Circles { get; set; }
    }
}