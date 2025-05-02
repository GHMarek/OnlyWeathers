using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OnlyWeathersApi.Models;
using OnlyWeathersApi.Services;

namespace OnlyWeathersApi.Tests.Services
{
    public class JwtServiceTests
    {
        /// <summary>
        /// Ten test sprawdza, czy metoda GenerateToken z serwisu JwtService generuje poprawny token JWT 
        /// zawierający claim z adresem e-mail użytkownika (ClaimTypes.Name).
        /// </summary>
        [Fact]
        public void GenerateToken_ShouldContainUserEmailClaim()
        {
            // --- Arrange ---

            // Tworzymy mockowane ustawienia JWT (klucz + issuer)
            var settings = Options.Create(new JwtSettings
            {
                SecretKey = "tajemniczy_jwt_tajemniczy_jwt_key_123456", // klucz symetryczny
                Issuer = "testissuer" // issuer nie jest tu testowany, ale musi być ustawiony
            });

            // Tworzymy serwis JWT z ustawieniami
            var service = new JwtService(settings);

            // --- Act ---

            // Generujemy token dla użytkownika
            var token = service.GenerateToken(new User
            {
                Id = 1,
                Email = "user@test.com",
                Role = "User"
            });

            // Dekodujemy wygenerowany JWT (parsujemy jako JwtSecurityToken)
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            // --- Assert ---

            // Sprawdzamy, czy wśród claimów jest ten z typem "Name" i wartością "user@test.com"
            Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.Name && c.Value == "user@test.com");
        }
    }
}
