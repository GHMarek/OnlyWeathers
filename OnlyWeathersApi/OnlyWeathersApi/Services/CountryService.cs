using System.Net.Http.Json;
using OnlyWeathersApi.Models.DTO;

namespace OnlyWeathersApi.Services
{
    /// <summary>
    /// Serwis odpowiedzialny za integrację z API RestCountries
    /// </summary>
    public class CountryService : ICountryService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public CountryService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        /// <summary>
        /// Pobiera wszystkie kraje i zwraca listę stolic z dodatkowymi informacjami.
        /// </summary>
        /// <returns></returns>
        public async Task<List<(string Capital, string Country, string Flag, string CountryCode)>> GetCapitalCitiesAsync()
        {
            var baseUrl = _config["RestCountries:BaseUrl"];
            var countries = await _httpClient.GetFromJsonAsync<List<CountryDto>>($"{baseUrl}/all");

            return countries!
                .Where(c => c.Capital != null && c.Capital.Any())
                .Select(c => (
                    Capital: c.Capital!.First(),
                    Country: c.Name.Common,
                    Flag: c.Flags.Png,
                    CountryCode: c.Cca2
                ))
                .ToList();
        }

        /// <summary>
        /// Pobiera tylko kraje europejskie i ich stolice z dodatkowymi informacjami.
        /// </summary>
        /// <returns></returns>
        public async Task<List<(string Capital, string Country, string Flag, string CountryCode)>> GetCapitalCitiesEuropeAsync()
        {
            var baseUrl = _config["RestCountries:BaseUrl"];
            var countries = await _httpClient.GetFromJsonAsync<List<CountryDto>>($"{baseUrl}/region/europe");

            return countries!
                .Where(c => c.Capital != null && c.Capital.Any())
                .Select(c => (
                    Capital: c.Capital!.First(),
                    Country: c.Name.Common,
                    Flag: $"https://flagcdn.com/w40/{c.Cca2.ToLower()}.png",
                    CountryCode: c.Cca2
                ))
                .ToList();
        }

    }
}
