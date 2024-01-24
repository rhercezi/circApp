using Core.Messages;

namespace User.Query.Application.Queries
{
    public class LoginQuery : BaseQuery
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public LoginQuery(string username, string password)
        {
            Password = password;
            Username = username;
        }
    }
}