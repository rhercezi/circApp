using Core.Messages;

namespace User.Query.Application.Queries
{
    public class RefreshTokenQuery : BaseQuery
    {
        public string? RefreshToken { get; set; }
    }
}