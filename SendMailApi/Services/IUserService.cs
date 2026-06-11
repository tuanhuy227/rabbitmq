using SendMailApi.DTOs;

namespace SendMailApi.Services
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(RegisterDto registerDto);
    }
}
