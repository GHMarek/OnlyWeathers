using OnlyWeathersApi.Models.DTO;

namespace OnlyWeathersApi.Services
{
    public interface IFavoriteService
    {
        Task<List<WeatherDto>> GetFavoriteWeatherAsync(int userId);
        Task<List<FavoriteCityDto>> GetFavoritesAsync(int userId);
        Task<(bool Success, string? Error)> AddFavoriteAsync(int userId, string cityName);
        Task<bool> DeleteFavoriteAsync(int userId, int favoriteId);
        Task<bool> UpdateAliasAsync(int userId, int favoriteId, string alias);

    }
}
