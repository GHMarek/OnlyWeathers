namespace OnlyWeathersApi.Models.DTO
{
    public class WeatherDto
    {
        public string City { get; set; } = null!;
        public string Description { get; set; } = null!;
        public double Temperature { get; set; }
        public int Humidity { get; set; }
        public double WindSpeed { get; set; }
    }
}
