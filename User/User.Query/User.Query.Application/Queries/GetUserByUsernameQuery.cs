using Core.Messages;

namespace User.Query.Application.Queries
{
    public class GetUserByUsernameQuery : BaseQuery
    {
        public string Username { get; set; }

        public GetUserByUsernameQuery(string username)
        {
            Username = username;
        }
    }
}