using System.Text.Json;
using OnlyWeathersApi.Models.DTO;

namespace OnlyWeathersApi.Services
{
    public class GeoDbService : IGeoDbService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public GeoDbService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<List<CityDto>> SearchCitiesAsync(string query)
        {
            var baseUrl = _config["GeoDb:BaseUrl"];
            var apiKey = _config["GeoDb:ApiKey"];
            var apiHost = _config["GeoDb:ApiHost"];

            var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/cities?namePrefix={query}&types=CITY&limit=10");

            request.Headers.Add("X-RapidAPI-Key", apiKey);
            request.Headers.Add("X-RapidAPI-Host", apiHost);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode) return new();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var data = doc.RootElement.GetProperty("data");

            return data.EnumerateArray()
            .Select(c => new CityDto
            {
                City = c.GetProperty("city").GetString() ?? "",
                Country = c.GetProperty("country").GetString() ?? "",
                CountryCode = c.GetProperty("countryCode").GetString() ?? "",
                Region = c.TryGetProperty("region", out var regionProp) ? regionProp.GetString() ?? "" : "",
                Latitude = c.GetProperty("latitude").GetDouble(),
                Longitude = c.GetProperty("longitude").GetDouble()
            })
            .ToList();
        }

    }

}
