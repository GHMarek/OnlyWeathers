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
        private readonly IWeatherService _weatherService;
        private readonly IGeoDbService _geoDbService;

        public UsersController(IUserService userService, IWeatherService weatherService, IGeoDbService geoDbService)
        {
            _userService = userService;
            _weatherService = weatherService;
            _geoDbService = geoDbService;
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

        [Authorize]
        [HttpGet("favorites")]
        public async Task<IActionResult> GetFavoriteCitiesWeather()
        {
            var email = User.Identity?.Name;
            var user = await _userService.GetUserByEmailAsync(email!);
            if (user == null)
                return Unauthorized();

            var weatherList = new List<object>();

            foreach (var favCity in user.FavoriteCities)
            {
                var weather = await _weatherService.GetWeatherAsync(favCity.CityName);
                if (weather != null)
                {
                    weatherList.Add(new
                    {
                        city = favCity.CityName,
                        temperature = weather.Temperature,
                        description = weather.Description,
                        icon = weather.Icon
                    });
                }
            }

            return Ok(weatherList);
        }

        [Authorize]
        [HttpPost("favorites")]
        public async Task<IActionResult> AddFavoriteCity([FromBody] string cityName)
        {
            var email = User.Identity?.Name;
            var success = await _userService.AddFavoriteCityAsync(email!, cityName);

            if (!success)
                return BadRequest("City already added.");

            return Ok("City added to favorites.");
        }


        [Authorize]
        [HttpGet("cities")]
        public async Task<IActionResult> SearchCities([FromQuery] string query)
        {
            var results = await _geoDbService.SearchCitiesAsync(query);
            return Ok(results);
        }

    }
}
