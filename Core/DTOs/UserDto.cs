using System.Text.Json.Serialization;

namespace Core.DTOs
{
    public class UserDto : BaseDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("created")]
        public DateTime? Created { get; set; }
        [JsonPropertyName("updated")]
        public DateTime? Updated { get; set; }
        [JsonPropertyName("userName")]
        public string? UserName { get; set; }
        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }
        [JsonPropertyName("familyName")]
        public string? FamilyName { get; set; }
        [JsonPropertyName("email")]
        public string? Email { get; set; }
        [JsonPropertyName("emailVerified")]
        public bool EmailVerified { get; set; }
    }
}