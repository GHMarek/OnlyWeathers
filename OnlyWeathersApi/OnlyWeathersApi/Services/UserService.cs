using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using OnlyWeathersApi.Data;
using OnlyWeathersApi.Models;
using OnlyWeathersApi.Services;

namespace OnlyWeathersAPI.Services
{
    /// <summary>
    /// Serwis do zarządzania funkcjami użytkownika
    /// </summary>
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IGeoDbService _geoDbService;

        public UserService(AppDbContext context, IGeoDbService geoDbService)
        {
            _context = context;
            _geoDbService = geoDbService;
        }

        /// <summary>
        /// Zmienia hasło użytkownika
        /// </summary>
        /// <param name="email"></param>
        /// <param name="currentPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public async Task<bool> ChangePasswordAsync(string email, string currentPassword, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
                return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Rejestruje nowego użytkownika
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<RegisterResult> RegisterAsync(string email, string password)
        {
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (string.IsNullOrEmpty(email) || !Regex.IsMatch(email, emailPattern))
                return RegisterResult.InvalidEmail;

            if (string.IsNullOrEmpty(password))
                return RegisterResult.EmptyPassword;

            if (await _context.Users.AnyAsync(u => u.Email == email))
                return RegisterResult.UserExists;

            var hashed = BCrypt.Net.BCrypt.HashPassword(password);
            _context.Users.Add(new User
            {
                Email = email,
                PasswordHash = hashed,
                Role = "User",
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return RegisterResult.Success;
        }


        /// <summary>
        /// Pobiera użytkownika na podstawie adresu email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.FavoriteCities)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public enum RegisterResult
        {
            Success,
            InvalidEmail,
            EmptyPassword,
            UserExists
        }


    }
}
