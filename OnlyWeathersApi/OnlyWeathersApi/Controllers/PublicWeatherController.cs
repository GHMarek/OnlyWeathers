using Microsoft.AspNetCore.Mvc;
using OnlyWeathersApi.Services;

namespace OnlyWeathersApi.Controllers
{
    [ApiController]
    [Route("api/public-weather")]
    public class PublicWeatherController : ControllerBase
    {
        private readonly ICountryService _countryService;
        private readonly IWeatherService _weatherService;
        private readonly int countryCount;

        public PublicWeatherController(ICountryService countryService, IWeatherService weatherService)
        {
            _countryService = countryService;
            _weatherService = weatherService;
            countryCount = 10;
        }

        [HttpGet]
        public async Task<IActionResult> GetRandomWeather()
        {
            var capitals = await _countryService.GetCapitalCitiesEuropeAsync();
            var randomCapitals = capitals.OrderBy(_ => Guid.NewGuid()).Take(countryCount).ToList();

            var weatherTasks = randomCapitals.Select(async capital =>
            {
                var (city, country, flag, countryCode) = capital;
                var weather = await _weatherService.GetWeatherAsync(city);

                if (weather != null)
                {
                    return new
                    {
                        city,
                        country,
                        flag,
                        countryCode,
                        temperature = weather.Temperature,
                        description = weather.Description,
                        icon = weather.Icon
                    };
                }

                return null;
            });

            var weatherResults = await Task.WhenAll(weatherTasks); // Odpalamy wszystkie zapytania równolegle

            // Filtrujemy null-e i bierzemy tylko 10 pierwszych wyników z pogodą
            var weatherList = weatherResults
                .Where(x => x != null)
                .Take(countryCount)
                .ToList();

            return Ok(weatherList);
        }
    }

}
