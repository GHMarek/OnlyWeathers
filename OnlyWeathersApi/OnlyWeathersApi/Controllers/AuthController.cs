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

        public AuthController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            //TODO: Na razie login na sztywno
            if (request.Email == "string" && request.Password == "string")
            {
                var token = _jwtService.GenerateToken(request.Email);
                return Ok(new { token });
            }

            return Unauthorized("Invalid credentials");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var success = await _userService.RegisterAsync(request.Email, request.Password);
            if (!success)
                return BadRequest("User already exists.");

            return Ok("User registered successfully.");
        }

    }
}
