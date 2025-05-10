using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using OnlyWeathersApi.Data;
using OnlyWeathersApi.Models;
using OnlyWeathersApi.Services;
using OnlyWeathersAPI.Services;
using static OnlyWeathersAPI.Services.UserService;

namespace OnlyWeathersApi.Tests.Services
{
    public class UserServiceTests
    {
        /// <summary>
        /// Test sprawdza, czy metoda RegisterAsync w UserService odrzuca próbę rejestracji nowego użytkownika,
        /// jeśli adres e-mail jest już zajęty.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task RegisterAsync_ShouldReturnUserExists_WhenEmailExists()
        {
            // --- Arrange ---
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("register_fail_test")
                .Options;

            using var context = new AppDbContext(options);

            // Dodajemy użytkownika z istniejącym adresem e-mail
            context.Users.Add(new User
            {
                Email = "test@test.com",
                PasswordHash = "dummy",
                Role = "User",
                CreatedAt = DateTime.UtcNow
            });

            await context.SaveChangesAsync();

            var service = new UserService(context, Mock.Of<IGeoDbService>());

            // --- Act ---
            var result = await service.RegisterAsync("test@test.com", "password");

            // --- Assert ---
            Assert.Equal(RegisterResult.UserExists, result);
        }


    }

}
