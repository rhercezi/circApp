using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Core.DTOs
{
    [BsonIgnoreExtraElements]
    public class CircleDto : BaseDto
    {
        public CircleDto()
        {
            Users = new List<AppUserDto>();
        }
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        
        [JsonPropertyName("creatorId")]
        public Guid CreatorId { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("color")]
        public string? Color { get; set; }
        [JsonPropertyName("users")]
        public List<AppUserDto> Users { get; set; }
    }
}