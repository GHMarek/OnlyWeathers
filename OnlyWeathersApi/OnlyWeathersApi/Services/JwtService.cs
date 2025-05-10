using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OnlyWeathersApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnlyWeathersApi.Services
{
    /// <summary>
    /// Serwis do generowania tokenów JWT
    /// </summary>
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        /// <summary>
        /// Generuje token JWT dla użytkownika.
        /// Nadaje on tokenie następujące claimy:
        /// NameIdentifier (Id użytkownika), Name (e-mail użytkownika), Role (rola użytkownika).
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                // TODO: załóżmy, że rola będzie dodana w przyszłości
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                issuer: _jwtSettings.Issuer,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
