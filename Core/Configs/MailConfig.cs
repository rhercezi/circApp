namespace Core.Configs
{
    public class MailConfig
    {
        public MailConfig()
        {
        }

        public MailConfig(MailConfig config)
        {
            BaseUrl = config.BaseUrl;
            Server = config.Server;
            Port = config.Port;
            EnableSSL = config.EnableSSL;
            Sender = config.Sender;
            Company = config.Company;
            Username = config.Username;
            Password = config.Password;
            Subject = config.Subject;
            Body = new List<string>();
            config.Body?.ForEach(
                    b => Body.Add(b)
                );
        }

        public string? BaseUrl { get; set; }
        public string? Server { get; set; }
        public int Port { get; set; }
        public bool EnableSSL { get; set; }
        public string? Sender { get; set; }
        public string? Company { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Subject { get; set; }
        public List<string>? Body { get; set; }
    }
}