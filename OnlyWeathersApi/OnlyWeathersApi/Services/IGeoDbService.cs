using OnlyWeathersApi.Models.DTO;

namespace OnlyWeathersApi.Services
{
    public interface IGeoDbService
    {
        Task<List<CityDto>> SearchCitiesAsync(string query);
    }

}
