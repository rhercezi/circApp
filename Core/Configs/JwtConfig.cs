namespace Core.Configs
{
    public class JwtConfig
    {
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public required string SigningKey { get; set; }
    }
}