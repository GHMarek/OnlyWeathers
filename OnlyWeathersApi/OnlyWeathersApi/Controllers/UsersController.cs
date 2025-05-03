using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlyWeathersApi.Models.DTO;
using OnlyWeathersApi.Services;
using System.Security.Claims;
using static OnlyWeathersAPI.Services.UserService;

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
            var result = await _userService.RegisterAsync(request.Email, request.Password);

            return result switch
            {
                RegisterResult.Success => Ok("User registered successfully."),
                RegisterResult.InvalidEmail => BadRequest("Invalid email format."),
                RegisterResult.EmptyPassword => BadRequest("Password cannot be empty."),
                RegisterResult.UserExists => BadRequest("User with this email already exists."),
                _ => StatusCode(500, "Unknown registration error.")
            };
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
