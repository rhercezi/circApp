using Core.DTOs;

namespace User.Query.Application.DTOs
{
    public class LoginDto : BaseDto
    {
        public TokensDto? Tokens { get; set; }
        public UserDto? User { get; set; }
    }
}