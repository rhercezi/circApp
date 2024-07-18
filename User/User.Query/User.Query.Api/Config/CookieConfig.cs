namespace User.Query.Api.Config
{
    public class CookieConfig
    {
        public bool HttpOnly { get; set; }
        public bool Secure { get; set; }
        public required string SameSite { get; set; }
        public int AccessMaxAge { get; set; }
        public int RefreshMaxAge { get; set; }
    }
}