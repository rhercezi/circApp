using System.Text.Json.Serialization;

namespace Core.DTOs
{
    public class AppUsersDto : BaseDto
    {
        public AppUsersDto()
        {
            Users = new List<AppUserDto>();
        }
        [JsonPropertyName("users")]
        public List<AppUserDto>? Users { get; set; }
    }
}