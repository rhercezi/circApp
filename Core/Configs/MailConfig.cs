namespace Core.Configs
{
    public class MailConfig
    {
        public string BaseUrl { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public bool EnableSSL { get; set; }
        public string Sender { get; set; }
        public string Company { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Subject { get; set; }
        public List<string> Body { get; set; }
    }
}