using System.Net.Http;
using System.Text.Json;
using OnlyWeathersApi.Models.DTO;

namespace OnlyWeathersApi.Services
{
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
            // W url decydujemy o jezyku i systemie miar
            var url = $"{baseUrl}weather?q={city}&appid={apiKey}&units=metric&lang=en";

            Console.WriteLine($"Request: {url}");

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"OpenWeather ERROR: {response.StatusCode} {error}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            try
            {
                return new WeatherDto
                {
                    City = root.GetProperty("name").GetString() ?? city,
                    Description = root.GetProperty("weather")[0].GetProperty("description").GetString() ?? "unknown",
                    Temperature = root.GetProperty("main").GetProperty("temp").GetDouble(),
                    Humidity = root.GetProperty("main").GetProperty("humidity").GetInt32(),
                    WindSpeed = root.GetProperty("wind").GetProperty("speed").GetDouble(),
                    Icon = root.GetProperty("weather")[0].GetProperty("icon").GetString() ?? ""
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing weather data: {ex.Message}");
                return null;
            }
        }

    }
}
