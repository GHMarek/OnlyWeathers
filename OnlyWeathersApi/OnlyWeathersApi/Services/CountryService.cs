using System.Net.Http.Json;

namespace OnlyWeathersApi.Services
{
    public class CountryService : ICountryService
    {
        private readonly HttpClient _httpClient;
        public CountryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<(string Capital, string Country, string Flag, string CountryCode)>> GetCapitalCitiesAsync()
        {
            var countries = await _httpClient.GetFromJsonAsync<List<CountryResponse>>("https://restcountries.com/v3.1/all");

            return countries
                .Where(c => c.Capital != null && c.Capital.Any())
                .Select(c => (
                    Capital: c.Capital.First(),
                    Country: c.Name.Common,
                    Flag: c.Flags.Png,
                    CountryCode: c.Cca2
                ))
                .ToList();
        }

        // Tylko europa dla optymalizacji
        public async Task<List<(string Capital, string Country, string Flag, string CountryCode)>> GetCapitalCitiesEuropeAsync()
        {
            var countries = await _httpClient.GetFromJsonAsync<List<CountryResponse>>(
                "https://restcountries.com/v3.1/region/europe" // tylko Europa!
            );

            return countries
                .Where(c => c.Capital != null && c.Capital.Any())
                .Select(c => (
                    Capital: c.Capital.First(),
                    Country: c.Name.Common,
                    Flag: c.Flags.Png,
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
