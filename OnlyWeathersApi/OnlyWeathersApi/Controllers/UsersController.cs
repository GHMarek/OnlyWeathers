using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    }
}
