using OnlyWeathersApi.Models.DTO;

namespace OnlyWeathersApi.Services
{
    public interface IWeatherService
    {
        Task<WeatherDto?> GetWeatherAsync(string city);
    }
}
