using System.Net.Http;
using System.Text.Json;
using OnlyWeathersApi.Models.DTO;

namespace OnlyWeathersApi.Services
{
    /// <summary>
    /// Serwis do pobierania danych pogodowych z OpenWeather
    /// </summary>
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public WeatherService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<WeatherDto?> GetWeatherAsync(string city)
        {
            var apiKey = _config["OpenWeather:ApiKey"];
            var baseUrl = _config["OpenWeather:BaseUrl"];
            var url = $"{baseUrl}weather?q={city}&appid={apiKey}&units=metric&lang=en";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Pobranie pierwszego obiektu weather
            var weatherElement = root.GetProperty("weather")[0];

            return new WeatherDto
            {
                City = root.GetProperty("name").GetString() ?? city,
                Description = weatherElement.GetProperty("description").GetString() ?? "unknown",
                Temperature = root.GetProperty("main").GetProperty("temp").GetDouble(),
                Humidity = root.GetProperty("main").GetProperty("humidity").GetInt32(),
                WindSpeed = root.GetProperty("wind").GetProperty("speed").GetDouble(),
                // Pełny url
                Icon = $"http://openweathermap.org/img/wn/{weatherElement.GetProperty("icon").GetString()}@2x.png"
            };
        }


    }
}
