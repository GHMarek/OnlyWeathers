using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using OnlyWeathersApi.Models.DTO;
using OnlyWeathersApi.Services;
using System.Security.Claims;

namespace OnlyWeathersAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPut("password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var email = User.FindFirstValue(ClaimTypes.Name);
            if (email == null) return Unauthorized();

            var result = await _userService.ChangePasswordAsync(email, request.CurrentPassword, request.NewPassword);
            if (!result) return BadRequest("Invalid current password.");

            return Ok("Password changed successfully.");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var success = await _userService.RegisterAsync(request.Email, request.Password);
            if (!success)
                return BadRequest("Invalid input or user already exists.");

            return Ok("User registered successfully.");
        }
    }
}
