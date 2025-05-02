using static OnlyWeathersApi.Services.CountryService;

namespace OnlyWeathersApi.Models.DTO
{
    public class CountryDto
    {
        public List<string>? Capital { get; set; }
        public NameDto Name { get; set; } = null!;
        public FlagsDto Flags { get; set; } = null!;
        public string Cca2 { get; set; } = null!;
    }
}
