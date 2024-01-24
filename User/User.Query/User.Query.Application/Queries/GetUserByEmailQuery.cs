using Core.Messages;

namespace User.Query.Application.Queries
{
    public class GetUserByEmailQuery : BaseQuery
    {
        public string Email { get; set; }

        public GetUserByEmailQuery(string email)
        {
            Email = email;
        }
    }
}