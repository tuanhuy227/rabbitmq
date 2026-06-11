using SendMailApi.DTOs;

namespace SendMailApi.Services
{
    public interface IRabbitMQService
    {
        void SendEmailMessage(EmailMessageDto emailMessage);
    }
}
