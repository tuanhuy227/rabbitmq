using Microsoft.EntityFrameworkCore;
using SendMailApi.Data;
using SendMailApi.DTOs;
using SendMailApi.Models;

namespace SendMailApi.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IRabbitMQService _rabbitMQService;

        public UserService(AppDbContext context, IRabbitMQService rabbitMQService)
        {
            _context = context;
            _rabbitMQService = rabbitMQService;
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email);
            if (existingUser != null)
            {
                return false;
            }

            var user = new User
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var emailMessage = new EmailMessageDto
            {
                To = user.Email,
                Subject = "Chào mừng bạn đã đăng ký!",
                Body = $@"
                    <h1>Xin chào {user.FullName}!</h1>
                    <p>Cảm ơn bạn đã đăng ký tài khoản tại ứng dụng của chúng tôi.</p>
                    <p>Chúc bạn có trải nghiệm tốt đẹp!</p>
                "
            };

            _rabbitMQService.SendEmailMessage(emailMessage);

            return true;
        }
    }
}
