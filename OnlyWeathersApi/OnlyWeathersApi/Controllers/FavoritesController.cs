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

        [HttpGet("weather")]
        public async Task<IActionResult> GetWeather()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _favoriteService.GetFavoriteWeatherAsync(userId);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _favoriteService.GetFavoritesAsync(userId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddFavorite([FromBody] AddFavoriteDto request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var (success, error) = await _favoriteService.AddFavoriteAsync(userId, request.CityName);

            if (!success)
                return BadRequest(error);

            return Ok("City added to favorites.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFavorite(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _favoriteService.DeleteFavoriteAsync(userId, id);

            return success ? Ok("City removed from favorites.") : NotFound();
        }

        [HttpPut("{id}/alias")]
        public async Task<IActionResult> UpdateAlias(int id, [FromBody] AliasUpdateDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _favoriteService.UpdateAliasAsync(userId, id, dto.Alias);

            return success ? Ok() : BadRequest("Could not update alias.");
        }


        [HttpGet("cities")]
        public async Task<IActionResult> SearchCities([FromQuery] string query)
        {
            var result = await _geoDbService.SearchCitiesAsync(query);
            return Ok(result);
        }
    }

}
