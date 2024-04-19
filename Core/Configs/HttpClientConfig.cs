namespace Core.Configs
{
    public class HttpClientConfig
    {
        public required string BaseUrl { get; set; }
        public required string Path { get; set; }
        public required int Port { get; set; }
    }
}