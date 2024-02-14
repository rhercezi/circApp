using System.Net;
using System.Net.Mail;
using Core.Configs;

namespace Core.Utilities
{
    public class EmailSenderService
    {
        public void SendMail(string idLink, string email, MailConfig config, int bodyIndex = 0)
        {

            var body = config.Body;

            string smtpServer = config.Server;
            int smtpPort = config.Port;

            using var client = new SmtpClient(smtpServer, smtpPort);
            client.Credentials = new NetworkCredential("testUser", "testPass");
            client.EnableSsl = config.EnableSSL;

            var message = new MailMessage
            {
                From = new MailAddress(config.Sender, config.Company),
                Subject = config.Subject,
                Body = body[bodyIndex],
                IsBodyHtml = true,
            };

            message.To.Add(email);
            client.Send(message);
        }
    }
}