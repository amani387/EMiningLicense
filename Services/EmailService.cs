using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EMiningLicense.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var settings = _config.GetSection("EmailSettings");

                var fromEmail = settings["FromEmail"];
                var fromPassword = settings["Password"];
                var host = settings["Host"];
                var port = int.Parse(settings["Port"]);
                var enableSsl = bool.Parse(settings["EnableSsl"]);

                using var client = new SmtpClient(host, port)
                {
                    EnableSsl = enableSsl,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromEmail, fromPassword)
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, settings["FromName"]),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Email sending failed: " + ex.Message);
                throw; // bubble up to AccountController
            }
        }

    }
}
