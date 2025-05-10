using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OnlyWeathersApi.Controllers
{
    /// <summary>
    /// Kontroler testowy, do łatwego sprawdzania autoryzacji w Swagger/Postman
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("private")]
        [Authorize]
        public IActionResult Private()
        {
            var email = User.FindFirstValue(ClaimTypes.Name);
            return Ok(new { message = $"Hello, {email}! You are authorized." });
        }

        [HttpGet("public")]
        public IActionResult Public()
        {
            return Ok("This is a public endpoint. No token required.");
        }
    }
}
