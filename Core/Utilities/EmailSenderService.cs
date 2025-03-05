using System.Net;
using System.Net.Mail;
using Core.Configs;

namespace Core.Utilities
{
    public class EmailSenderService
    {
        public void SendMail(string email, MailConfig config, int bodyIndex = 0)
        {
            string smtpServer = config.Server;
            int smtpPort = config.Port;

            using var client = new SmtpClient(smtpServer, smtpPort);
            client.Credentials = new NetworkCredential(config.Username, config.Password);
            client.EnableSsl = config.EnableSSL;

            var message = new MailMessage
            {
                From = new MailAddress(config.Sender, config.Company),
                Subject = config.Subject,
                Body = config.Body[bodyIndex],
                IsBodyHtml = true,
            };

            message.To.Add(email);
            client.Send(message);
        }
    }
}