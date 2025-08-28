using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var smtp = new SmtpClient
        {
            Host = _config["Smtp:Host"],       // e.g., smtp.gmail.com
            Port = int.Parse(_config["Smtp:Port"]),
            EnableSsl = true,
            Credentials = new NetworkCredential(
                _config["Smtp:Username"],
                _config["Smtp:Password"])
        };

        var mail = new MailMessage
        {
            From = new MailAddress(_config["Smtp:From"]),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mail.To.Add(toEmail);

        await smtp.SendMailAsync(mail);
    }
}
