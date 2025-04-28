using Microsoft.AspNetCore.Mvc;
using WordMemoryApi.DTOs;
using WordMemoryApi.Services;

namespace WordMemoryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly JwtService _jwtService;

        public AuthController(AuthService authService, JwtService jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);

            if (!result)
                return BadRequest("Bu kullanıcı adı zaten kullanılıyor!");

            return Ok("Kayıt başarılı!");
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _authService.LoginAsync(loginDto);

            if (user == null)
                return Unauthorized("Kullanıcı adı veya şifre hatalı!");

            var token = _jwtService.GenerateToken(user);

            return Ok(new { token });
        }
    }
}
