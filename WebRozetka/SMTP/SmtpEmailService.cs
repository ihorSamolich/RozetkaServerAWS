using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace WebRozetka.SMTP
{
    public class SmtpEmailService
    {
        private readonly EmailConfiguration _configuration;

        public SmtpEmailService()
        {
            _configuration = new EmailConfiguration();
        }
        public void Send(Message messageData)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("WebRozetka", _configuration.From));
            message.To.Add(new MailboxAddress(messageData.Name, messageData.To));
            message.Subject = "Web Rozerka order";

            message.Body = new TextPart("plain")
            {
                Text = messageData.Body
            };

            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_configuration.SmtpServer, _configuration.Port, SecureSocketOptions.SslOnConnect);    //-UkrNet
                    client.Authenticate(_configuration.UserName, _configuration.Password);
                    client.Send(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error sending: " + ex);
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }
    }
}
