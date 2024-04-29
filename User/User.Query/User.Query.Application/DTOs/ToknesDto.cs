using Core.DTOs;

namespace User.Query.Application.DTOs
{
    public class ToknesDto : BaseDto
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}