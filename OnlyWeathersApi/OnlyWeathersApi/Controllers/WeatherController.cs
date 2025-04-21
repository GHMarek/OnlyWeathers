using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OnlyWeathersApi.Services;

namespace OnlyWeathersApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet("{city}")]
        [Authorize]
        public async Task<IActionResult> GetWeather(string city)
        {
            var weather = await _weatherService.GetWeatherAsync(city);
            if (weather == null) return NotFound("City not found or API error.");

            return Ok(weather);
        }
    }
}
