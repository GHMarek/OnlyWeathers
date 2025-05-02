using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlyWeathersApi.Models.DTO;
using OnlyWeathersApi.Services;
using OnlyWeathersAPI.Services;

namespace OnlyWeathersApi.Controllers
{
    /// <summary>
    /// Kontroler odpowiadający za autoryzację użytkownika (logowanie)
    /// </summary>
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

        /// <summary>
        /// POST: api/auth/login. Endpoint do logowania – przyjmuje dane z formularza (LoginDto)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            // Pobieranie użytkownika po e-mailu
            var user = await _userService.GetUserByEmailAsync(request.Email);

            // Sprawdzenie, czy użytkownik istnieje oraz czy hasło się zgadza
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials"); // Zwraca 401 jeśli logowanie nieudane

            // Generowanie tokena JWT dla zalogowanego użytkownika
            var token = _jwtService.GenerateToken(user);

            // Zwracamy token w odpowiedzi – klient może go użyć do dalszych zapytań
            return Ok(new { token });
        }

    }
}
