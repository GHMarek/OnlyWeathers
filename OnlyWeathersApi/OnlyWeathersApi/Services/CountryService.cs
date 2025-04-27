using System.Net.Http.Json;

namespace OnlyWeathersApi.Services
{
    public class CountryService : ICountryService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public CountryService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<List<(string Capital, string Country, string Flag, string CountryCode)>> GetCapitalCitiesAsync()
        {
            var baseUrl = _config["RestCountries:BaseUrl"];
            var countries = await _httpClient.GetFromJsonAsync<List<CountryResponse>>($"{baseUrl}/all");

            return countries!
                .Where(c => c.Capital != null && c.Capital.Any())
                .Select(c => (
                    Capital: c.Capital.First(),
                    Country: c.Name.Common,
                    Flag: c.Flags.Png,
                    CountryCode: c.Cca2
                ))
                .ToList();
        }

        public async Task<List<(string Capital, string Country, string Flag, string CountryCode)>> GetCapitalCitiesEuropeAsync()
        {
            var baseUrl = _config["RestCountries:BaseUrl"];
            var countries = await _httpClient.GetFromJsonAsync<List<CountryResponse>>($"{baseUrl}/region/europe");

            return countries!
                .Where(c => c.Capital != null && c.Capital.Any())
                .Select(c => (
                    Capital: c.Capital.First(),
                    Country: c.Name.Common,
                    Flag: $"https://flagcdn.com/w40/{c.Cca2.ToLower()}.png",
                    CountryCode: c.Cca2
                ))
                .ToList();
        }


        public class CountryResponse
        {
            public List<string>? Capital { get; set; }
            public NameResponse Name { get; set; } = null!;
            public FlagsResponse Flags { get; set; } = null!;
            public string Cca2 { get; set; } = null!; // dodajemy kod kraju
        }

        public class NameResponse
        {
            public string Common { get; set; } = null!;
        }

        public class FlagsResponse
        {
            public string Png { get; set; } = null!;
        }

    }
}
