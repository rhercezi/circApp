using System.Net;
using System.Net.Mail;
using Amazon.Runtime.Internal;
using Microsoft.Extensions.Options;
using User.Common.Events;

namespace User.Common.Utility
{
    public class EmailSenderService
    {
        private readonly MailConfig _config;
        public EmailSenderService(IOptions<MailConfig> config)
        {
            _config = config.Value;
        }

        public void SendMail(string idLink, UserCreatedEvent xEvent)
        {

            var body = _config.Body;
            body = body.Replace("[VerificationLink]", _config.BaseUrl + idLink);
            body = body.Replace("[User]", xEvent.FirstName + " " + xEvent.FamilyName);
            body = body.Replace("[Your Company]", _config.Company);

            string smtpServer = _config.Server;
            int smtpPort = _config.Port;

            using (var client = new SmtpClient(smtpServer, smtpPort))
            {
                client.Credentials = new NetworkCredential("testUser", "testPass");
                client.EnableSsl = _config.EnableSSL;

                var message = new MailMessage
                {
                    From = new MailAddress(_config.Sender, _config.Company),
                    Subject = _config.Subject,
                    Body = body,
                    IsBodyHtml = true,
                };

                message.To.Add(xEvent.Email);
                client.Send(message);
            }
        }
    }
}