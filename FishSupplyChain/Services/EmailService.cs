using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace FishSupplyChain.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration config;

        public EmailService(IConfiguration config)
        {
            this.config = config;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("hello@demomailtrap.co"));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            //await smtp.ConnectAsync(config["Email:SmtpHost"], int.Parse(config["Email:SmtpPort"]), true);
            await smtp.ConnectAsync(config["EmailProviderSettings:Host"], int.Parse(config["EmailProviderSettings:Port"]), SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(config["EmailProviderSettings:Username"], config["EmailProviderSettings:Password"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
