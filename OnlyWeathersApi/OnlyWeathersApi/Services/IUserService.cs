using OnlyWeathersApi.Models;

namespace OnlyWeathersApi.Services
{
    public interface IUserService
    {
        Task<bool> ChangePasswordAsync(string email, string currentPassword, string newPassword);
        Task<bool> RegisterAsync(string email, string password);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> AddFavoriteCityAsync(string email, string cityName); // do dodawania miasta
    }
}
