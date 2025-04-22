using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlyWeathersApi.Data;
using OnlyWeathersApi.Models.DTO;
using OnlyWeathersApi.Models;
using Microsoft.EntityFrameworkCore;

namespace OnlyWeathersApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FavoritesController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/favorites
        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var favorites = await _context.FavoriteCities
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

            return Ok(favorites);
        }

        // POST /api/favorites
        [HttpPost]
        public async Task<IActionResult> AddFavorite([FromBody] FavoriteCityDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // limit 10 miast
            var count = await _context.FavoriteCities.CountAsync(f => f.UserId == userId);
            if (count >= 10)
                return BadRequest("You can have a maximum of 10 favorite cities.");

            var favorite = new FavoriteCity
            {
                CityName = dto.CityName,
                CountryCode = dto.CountryCode,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                UserId = userId
            };

            _context.FavoriteCities.Add(favorite);
            await _context.SaveChangesAsync();

            return Ok("City added to favorites.");
        }

        // DELETE /api/favorites/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFavorite(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var favorite = await _context.FavoriteCities
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);

            if (favorite == null)
                return NotFound();

            _context.FavoriteCities.Remove(favorite);
            await _context.SaveChangesAsync();

            return Ok("City removed from favorites.");
        }
    }
}

