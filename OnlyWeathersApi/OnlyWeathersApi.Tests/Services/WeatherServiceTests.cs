using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq.Protected;
using Moq;
using OnlyWeathersApi.Services;

namespace OnlyWeathersApi.Tests.Services
{
    public class WeatherServiceTests
    {
        /// <summary>
        /// Ten test sprawdza, czy metoda GetWeatherAsync w serwisie WeatherService 
        /// prawidłowo przetwarza dane z API pogodowego OpenWeather i zwraca poprawnie 
        /// zmapowany obiekt WeatherDto.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetWeatherAsync_Returns_WeatherDto_When_Valid()
        {
            // --- Arrange ---

            // Przykładowa odpowiedź JSON z OpenWeather (w wersji uproszczonej do testów)
            // Ikony: https://openweathermap.org/weather-conditions
            var jsonResponse = """
            {
              "name": "Berlin",
              "weather": [{"description": "clear sky", "icon": "01d"}],
              "main": {"temp": 20.0, "humidity": 50},
              "wind": {"speed": 3.5}
            }
            """;

            // Mock handlera HTTP – symuluje odpowiedź z zewnętrznego API pogodowego
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            // Tworzenie HttpClienta z podmienionym handlerem (bez realnych zapytań sieciowych)
            var client = new HttpClient(handlerMock.Object);

            // Konfiguracja serwisu pogodowego (podstawowe ustawienia potrzebne do działania)
            var configMock = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"OpenWeather:ApiKey", "abc"},
                    {"OpenWeather:BaseUrl", "https://api.openweathermap.org/data/2.5/"}
                })
                .Build();

            var service = new WeatherService(client, configMock);

            // --- Act ---
            // Wywołanie testowanej metody z nazwą miasta
            var result = await service.GetWeatherAsync("Berlin");

            // --- Assert ---
            Assert.NotNull(result);
            Assert.Equal("Berlin", result.City);
            Assert.Equal("clear sky", result.Description);
            Assert.Equal(20.0, result.Temperature);
            Assert.Equal(50, result.Humidity);
            Assert.Equal(3.5, result.WindSpeed);
            Assert.Contains("01d", result.Icon);
        }
    }
}
