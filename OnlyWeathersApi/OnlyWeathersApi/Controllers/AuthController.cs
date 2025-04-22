using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlyWeathersApi.Models.DTO;
using OnlyWeathersApi.Services;
using OnlyWeathersAPI.Services;

namespace OnlyWeathersApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly IUserService _userService;

        public AuthController(JwtService jwtService, IUserService userService)
        {
            _jwtService = jwtService;
            _userService = userService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.GetUserByEmailAsync(request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials");

            var token = _jwtService.GenerateToken(user);
            return Ok(new { token });
        }

    }
}
