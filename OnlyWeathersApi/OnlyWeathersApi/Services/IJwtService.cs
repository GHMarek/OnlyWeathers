using OnlyWeathersApi.Models;

namespace OnlyWeathersApi.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);


    }
}
