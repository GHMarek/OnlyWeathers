namespace OnlyWeathersApi.Models.DTO
{
    public class FavoriteCityDto
    {
        public int Id { get; set; }
        public string CityName { get; set; } = null!;
        public string CountryCode { get; set; } = null!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
