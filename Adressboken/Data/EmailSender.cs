using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Adressboken.Data
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            this.emailSettings = emailSettings?.Value ?? throw new ArgumentNullException(nameof(emailSettings));
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Email address cannot be null or empty.", nameof(email));
            }

            Console.WriteLine($"Email address: {email}");
            Console.WriteLine($"SenderEmail: {emailSettings.SenderEmail}");
            Console.WriteLine($"SenderName: {emailSettings.SenderName}");

            var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(emailSettings.SenderEmail, emailSettings.SenderName);
            mailMessage.Subject = subject;
            mailMessage.Body = message;
            mailMessage.IsBodyHtml = true;

            if (!string.IsNullOrEmpty(email))
            {
                mailMessage.To.Add(new MailAddress(email));
            }

            Console.WriteLine($"MailMessage: From={mailMessage.From}, To={mailMessage.To}");

            if (mailMessage.To.Count > 0)
            {
                using (var client = new SmtpClient(emailSettings.Host, emailSettings.Port))
                {
                    client.Credentials = new NetworkCredential(emailSettings.UserName, emailSettings.Password);
                    client.EnableSsl = true;
                    await client.SendMailAsync(mailMessage);
                }
            }
            else
            {
                throw new ArgumentException("Invalid email address.", nameof(email));
            }
        }
    }
}
