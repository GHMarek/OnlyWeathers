using Microsoft.EntityFrameworkCore;
using OnlyWeathersApi.Data;
using OnlyWeathersApi.Models;
using OnlyWeathersApi.Models.DTO;

namespace OnlyWeathersApi.Services
{
    /// <summary>
    /// Serwis do zarządzania ulubionymi miastami użytkownika
    /// </summary>
    public class FavoriteService : IFavoriteService
    {
        private readonly AppDbContext _context;
        private readonly IGeoDbService _geoDbService;
        private readonly IWeatherService _weatherService;

        public FavoriteService(AppDbContext context, IGeoDbService geoDbService, IWeatherService weatherService)
        {
            _context = context;
            _geoDbService = geoDbService;
            _weatherService = weatherService;
        }

        /// <summary>
        /// Zwraca pogodę dla ulubionych miast użytkownika
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<WeatherDto>> GetFavoriteWeatherAsync(int userId)
        {
            var favorites = await _context.FavoriteCities
                .Where(f => f.UserId == userId)
                .ToListAsync();

            var result = new List<WeatherDto>();

            foreach (var fav in favorites)
            {
                var weather = await _weatherService.GetWeatherAsync(fav.CityName);

                result.Add(new WeatherDto
                {
                    Id = fav.Id,
                    City = fav.CityName,
                    Temperature = weather?.Temperature,
                    Description = weather?.Description ?? "No weather data",
                    Icon = weather?.Icon ?? "/icons/not-available.png",
                    Alias = fav.Alias
                });
            }

            return result;
        }

        /// <summary>
        /// Zwraca listę ulubionych miast użytkownika
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<FavoriteCityDto>> GetFavoritesAsync(int userId)
        {
            return await _context.FavoriteCities
                .Where(f => f.UserId == userId)
                .Select(f => new FavoriteCityDto
                {
                    Id = f.Id,
                    CityName = f.CityName,
                    CountryCode = f.CountryCode,
                    Latitude = f.Latitude,
                    Longitude = f.Longitude
                })
                .ToListAsync();
        }

        /// <summary>
        /// Dodaje nowe miasto do ulubionych
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cityName"></param>
        /// <returns></returns>
        public async Task<(bool Success, string? Error)> AddFavoriteAsync(int userId, string cityName)
        {
            var user = await _context.Users
                .Include(u => u.FavoriteCities)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return (false, "Unauthorized");

            if (user.FavoriteCities.Count >= 10)
                return (false, "You can have a maximum of 10 favorite cities.");

            if (user.FavoriteCities.Any(fc => fc.CityName == cityName))
                return (false, "City already added.");

            var cities = await _geoDbService.SearchCitiesAsync(cityName);
            var city = cities.FirstOrDefault(c => c.City.Equals(cityName, StringComparison.OrdinalIgnoreCase));
            if (city == null) return (false, "City not found.");

            user.FavoriteCities.Add(new FavoriteCity
            {
                CityName = city.City,
                CountryCode = city.CountryCode,
                Latitude = city.Latitude,
                Longitude = city.Longitude,
                UserId = userId
            });

            await _context.SaveChangesAsync();
            return (true, null);
        }

        /// <summary>
        /// Usuwa ulubione miasto użytkownika
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="favoriteId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteFavoriteAsync(int userId, int favoriteId)
        {
            var fav = await _context.FavoriteCities
                .FirstOrDefaultAsync(f => f.Id == favoriteId && f.UserId == userId);

            if (fav == null) return false;

            _context.FavoriteCities.Remove(fav);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Aktualizuje alias ulubionego miasta
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="favoriteId"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAliasAsync(int userId, int favoriteId, string alias)
        {
            var fav = await _context.FavoriteCities
                .FirstOrDefaultAsync(f => f.Id == favoriteId && f.UserId == userId);

            if (fav == null)
                return false;

            fav.Alias = alias;
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
