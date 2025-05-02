namespace OnlyWeathersApi.Models.DTO
{
    public class WeatherDto
    {
        public int Id { get; set; }
        public string City { get; set; } = null!;
        public string Description { get; set; } = null!;
        public double? Temperature { get; set; }
        public int Humidity { get; set; }
        public double WindSpeed { get; set; }
        public string Icon { get; set; } = string.Empty;
        public string? Alias { get; set; }
    }
}
