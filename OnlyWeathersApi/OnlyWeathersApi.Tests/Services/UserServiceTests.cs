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
        public async Task RegisterAsync_ShouldReturnFalse_WhenEmailExists()
        {
            // --- Arrange ---

            // Tworzymy opcje dla bazy in-memory
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("register_fail_test")
                .Options;

            // Używamy contextu EF z in-memory DB (w zasięgu using, żeby wyczyścić po zakończeniu)
            using var context = new AppDbContext(options);

            // Dodajemy użytkownika z e-mailem, który ma być już zajęty
            context.Users.Add(new User
            {
                Email = "test@test.com",
                PasswordHash = "dummy",
                Role = "User",
                CreatedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            // Tworzymy instancję UserService bez potrzeby użycia GeoDbService (mock pusty)
            var service = new UserService(context, Mock.Of<IGeoDbService>());

            // --- Act ---

            // Próbujemy zarejestrować użytkownika z tym samym adresem e-mail
            var result = await service.RegisterAsync("test@test.com", "password");

            // --- Assert ---

            // Oczekujemy, że metoda zwróci false, bo taki e-mail już istnieje
            Assert.False(result);
        }

    }

}
