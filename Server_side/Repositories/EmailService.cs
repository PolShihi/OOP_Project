using System.Net.Mail;
using System.Net;

namespace Server_side.Repositories
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpConfig = _configuration.GetSection("Smtp");

            using (var smtpClient = new SmtpClient(smtpConfig["Host"]))
            {
                smtpClient.Port = int.Parse(smtpConfig["Port"]);
                smtpClient.Credentials = new NetworkCredential(smtpConfig["UserName"], smtpConfig["Password"]);
                smtpClient.EnableSsl = bool.Parse(smtpConfig["EnableSSL"]);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpConfig["UserName"]),
                    Subject = subject,
                    Body = "<html><body>" + body.Replace("\n", "<br>") + "</html></body>",
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(toEmail);

                try
                {
                    await smtpClient.SendMailAsync(mailMessage);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to send email", ex);
                }
            }
        }
    }
}
