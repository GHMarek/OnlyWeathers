using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlyWeathersApi.Models.DTO;
using OnlyWeathersApi.Services;
using System.Security.Claims;

namespace OnlyWeathersApi.Controllers
{
    /// <summary>
    /// Ten kontroler odpowiada za operacje związane z użytkownikami.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// POST: api/users/register
        /// Rejestracja nowego użytkownika
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            var success = await _userService.RegisterAsync(request.Email, request.Password);
            if (!success)
                return BadRequest("Invalid input or user already exists.");

            return Ok("User registered successfully.");
        }

        /// <summary>
        /// PUT: api/users/password
        /// Zmiana hasła – tylko dla zalogowanego użytkownika
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            var email = User.FindFirstValue(ClaimTypes.Name);
            if (email == null) return Unauthorized();

            var result = await _userService.ChangePasswordAsync(email, request.CurrentPassword, request.NewPassword);
            if (!result) return BadRequest("Invalid current password.");

            return Ok("Password changed successfully.");
        }
    }
}
