namespace OnlyWeathersApi.Services
{
    public interface ICountryService
    {
        Task<List<(string Capital, string Country, string Flag, string CountryCode)>> GetCapitalCitiesAsync();
        Task<List<(string Capital, string Country, string Flag, string CountryCode)>> GetCapitalCitiesEuropeAsync();
    }

}
