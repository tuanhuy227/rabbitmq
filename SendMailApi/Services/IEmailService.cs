using SendMailApi.DTOs;

namespace SendMailApi.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessageDto emailMessage);
    }
}
