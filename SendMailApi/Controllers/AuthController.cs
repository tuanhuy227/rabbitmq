using Microsoft.AspNetCore.Mvc;
using SendMailApi.DTOs;
using SendMailApi.Services;

namespace SendMailApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.RegisterAsync(registerDto);
            if (!result)
            {
                return BadRequest(new { message = "Email đã tồn tại" });
            }

            return Ok(new { message = "Đăng ký thành công, vui lòng kiểm tra email" });
        }
    }
}
