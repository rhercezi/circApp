using Core.DTOs;

namespace User.Query.Application.DTOs
{
    public class TokenDto :BaseDto
    {
        public required string Token { get; set; }
    }
}