namespace OnlyWeathersApi.Models
{
    public class FavoriteCity
    {
        public int Id { get; set; }
        public string CityName { get; set; } = null!;
        public string CountryCode { get; set; } = null!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        // Powiązanie z użytkownikiem
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
