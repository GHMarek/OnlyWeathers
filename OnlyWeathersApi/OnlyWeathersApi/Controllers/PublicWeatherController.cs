using Microsoft.AspNetCore.Mvc;
using OnlyWeathersApi.Services;

namespace OnlyWeathersApi.Controllers
{
    /// <summary>
    ///  Lontroler udostępnia publiczny (bez autoryzacji) endpoint, 
    ///  który zwraca pogodę w losowych stolicach europejskich państw.
    /// </summary>
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
        /// <summary>
        /// GET: api/public-weather
        /// Zwraca pogodę dla losowych stolic europejskich
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetRandomWeather()
        {
            // Pobieramy listę europejskich stolic
            var capitals = await _countryService.GetCapitalCitiesEuropeAsync();
            // Losujemy 10 unikalnych stolic
            var randomCapitals = capitals.OrderBy(_ => Guid.NewGuid()).Take(countryCount).ToList();

            // Dla każdej stolicy asynchronicznie pobieramy pogodę
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

            // Czekamy aż wszystkie zapytania do serwisu pogodowego się zakończą
            var weatherResults = await Task.WhenAll(weatherTasks);

            // Filtrujemy null i ograniczamy wynik do 10 elementów
            var weatherList = weatherResults
                .Where(x => x != null)
                .Take(countryCount)
                .ToList();

            return Ok(weatherList);
        }
    }

}
