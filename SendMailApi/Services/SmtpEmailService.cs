using MailKit.Net.Smtp;
using MimeKit;
using SendMailApi.DTOs;

namespace SendMailApi.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public SmtpEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(EmailMessageDto emailMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                _configuration["Smtp:FromName"],
                _configuration["Smtp:FromEmail"]));
            message.To.Add(new MailboxAddress("", emailMessage.To));
            message.Subject = emailMessage.Subject;

            message.Body = new TextPart("html")
            {
                Text = emailMessage.Body
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(
                    _configuration["Smtp:Host"],
                    int.Parse(_configuration["Smtp:Port"] ?? "587"),
                    MailKit.Security.SecureSocketOptions.StartTls);
                
                await client.AuthenticateAsync(
                    _configuration["Smtp:UserName"],
                    _configuration["Smtp:Password"]);
                
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
