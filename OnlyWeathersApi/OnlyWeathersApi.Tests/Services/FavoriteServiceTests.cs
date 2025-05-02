using Xunit;
using Moq;
using OnlyWeathersApi.Data;
using OnlyWeathersApi.Models;
using OnlyWeathersApi.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using OnlyWeathersApi.Models.DTO;

public class FavoriteServiceTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unikalna baza dla każdego testu
            .Options;

        return new AppDbContext(options);
    }
    /// <summary>
    /// Ten test sprawdza, czy metoda AddFavoriteAsync nie pozwoli na dodanie miasta, 
    /// które już znajduje się na liście ulubionych użytkownika.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddFavoriteAsync_ShouldFail_WhenCityExists()
    {
        // Arrange
        var db = GetDbContext();
        var geoMock = new Mock<IGeoDbService>();
        var weatherMock = new Mock<IWeatherService>();
        var service = new FavoriteService(db, geoMock.Object, weatherMock.Object);

        var user = new User
        {
            Id = 1,
            Email = "test@test.com",
            PasswordHash = "dummy",
            Role = "User",
        };

        db.Users.Add(user);
        db.SaveChanges();

        // Dodajemy osobno powiązane miasto
        db.FavoriteCities.Add(new FavoriteCity
        {
            CityName = "Warsaw",
            CountryCode = "PL",
            Latitude = 52.23,
            Longitude = 21.01,
            UserId = user.Id
        });
        db.SaveChanges();

        // Act
        var (success, error) = await service.AddFavoriteAsync(1, "Warsaw");

        // Assert
        Assert.False(success);
        Assert.Equal("City already added.", error);
    }

    /// <summary>
    /// Test sprawdza, czy metoda AddFavoriteAsync poprawnie dodaje nowe miasto,
    /// jeśli jeszcze nie istnieje w ulubionych użytkownika.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddFavoriteAsync_ShouldSucceed_WhenCityIsNew()
    {
        // Arrange
        var db = GetDbContext();
        var geoMock = new Mock<IGeoDbService>();
        var weatherMock = new Mock<IWeatherService>();
        var service = new FavoriteService(db, geoMock.Object, weatherMock.Object);

        geoMock.Setup(g => g.SearchCitiesAsync("Krakow")).ReturnsAsync(new List<CityDto> {
            new CityDto { City = "Krakow", CountryCode = "PL", Latitude = 50.06, Longitude = 19.94 }
        });

        db.Users.Add(new User
        {
            Id = 1,
            Email = "test@test.com",
            PasswordHash = "dummy",
            Role = "User",
            FavoriteCities = new List<FavoriteCity>()
        });

        db.SaveChanges();

        // Act
        var (success, error) = await service.AddFavoriteAsync(1, "Krakow");

        // Assert
        Assert.True(success);
        Assert.Null(error);
    }
}
