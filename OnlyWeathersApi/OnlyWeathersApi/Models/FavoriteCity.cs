namespace OnlyWeathersApi.Models
{
    public class FavoriteCity
    {
        public int Id { get; set; }
        public string CityName { get; set; } = null!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public int UserId { get; set; }  // Foreign Key
    }
}
