namespace Core.Configs
{
    public class JwtConfig
    {
        public required string Issuer { get; set; }
        public required string SigningKey { get; set; }
        public required int TokenExpiration { get; set; }
        public required int RefreshTokenExpiration { get; set; }
    }
}