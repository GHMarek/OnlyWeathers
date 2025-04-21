using Microsoft.AspNetCore.Identity;
using OnlyWeathersApi.Models;
using OnlyWeathersApi.Services;

namespace OnlyWeathersAPI.Services
{
    public class UserService : IUserService
    {
        // tu w przyszłości EF Core
        private static readonly List<User> _users = new()
        {
            new User { Id = 1, Email = "admin@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin") }
        };

        public Task<bool> ChangePasswordAsync(string email, string currentPassword, string newPassword)
        {
            var user = _users.FirstOrDefault(u => u.Email == email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
                return Task.FromResult(false);

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            return Task.FromResult(true);
        }

        public Task<bool> RegisterAsync(string email, string password)
        {
            var exists = _users.Any(u => u.Email == email);
            if (exists) return Task.FromResult(false);

            var hashed = BCrypt.Net.BCrypt.HashPassword(password);

            var newUser = new User
            {
                Id = _users.Max(u => u.Id) + 1,
                Email = email,
                PasswordHash = hashed,
                Role = "User",
                CreatedAt = DateTime.UtcNow
            };

            _users.Add(newUser);
            return Task.FromResult(true);
        }

    }
}
