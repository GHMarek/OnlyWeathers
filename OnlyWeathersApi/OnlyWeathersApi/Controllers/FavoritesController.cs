using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlyWeathersApi.Data;
using OnlyWeathersApi.Models;
using OnlyWeathersApi.Models.DTO;
using OnlyWeathersApi.Services;
using System.Security.Claims;

namespace OnlyWeathersApi.Controllers
{
    /// <summary>
    /// Kontroler odpowiadający za obsługę ulubionych miejsc użytkownika
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;
        private readonly IGeoDbService _geoDbService;

        public FavoritesController(IFavoriteService favoriteService, IGeoDbService geoDbService)
        {
            _favoriteService = favoriteService;
            _geoDbService = geoDbService;
        }

        /// <summary>
        /// GET: api/favorites/weather
        /// Zwraca aktualną pogodę dla wszystkich ulubionych miast użytkownika
        /// </summary>
        /// <returns></returns>
        [HttpGet("weather")]
        public async Task<IActionResult> GetWeather()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _favoriteService.GetFavoriteWeatherAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// GET: api/favorites
        /// Zwraca listę ulubionych miast aktualnego użytkownika
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _favoriteService.GetFavoritesAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// POST: api/favorites
        /// Dodaje nowe miasto do ulubionych na podstawie nazwy
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddFavorite([FromBody] AddFavoriteDto request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var (success, error) = await _favoriteService.AddFavoriteAsync(userId, request.CityName);

            if (!success)
                return BadRequest(error);

            return Ok("City added to favorites.");
        }

        /// <summary>
        /// DELETE: api/favorites/{id}
        /// Usuwa miasto z ulubionych po ID wpisu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFavorite(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _favoriteService.DeleteFavoriteAsync(userId, id);

            return success ? Ok("City removed from favorites.") : NotFound();
        }

        /// <summary>
        /// PUT: api/favorites/{id}/alias
        /// Aktualizuje alias (własną nazwę) dla miasta w ulubionych
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{id}/alias")]
        public async Task<IActionResult> UpdateAlias(int id, [FromBody] AliasUpdateDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _favoriteService.UpdateAliasAsync(userId, id, dto.Alias);

            return success ? Ok() : BadRequest("Could not update alias.");
        }

        /// <summary>
        /// GET: api/favorites/cities?query=...
        /// Wyszukuje miasta po nazwie (do dodania ich do ulubionych)
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("cities")]
        public async Task<IActionResult> SearchCities([FromQuery] string query)
        {
            var result = await _geoDbService.SearchCitiesAsync(query);
            return Ok(result);
        }
    }

}
