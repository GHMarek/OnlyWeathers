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
    /// <summary>
    /// Testy jednostkowe dla serwisu GeoDbService
    /// </summary>
    public class GeoDbServiceTests
    {
        [Fact]
        public async Task SearchCitiesAsync_Returns_Cities_When_Valid()
        {
            // Arrange
            var jsonResponse = """
            {
              "data": [
                {
                  "city": "Paris",
                  "country": "France",
                  "countryCode": "FR",
                  "region": "Ile-de-France",
                  "latitude": 48.8566,
                  "longitude": 2.3522
                }
              ]
            }
            """;

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            // Tworzenie klienta HTTP z mockowanym handlerem
            var client = new HttpClient(handlerMock.Object);

            // Mockowana konfiguracja – zawiera dane potrzebne do połączenia z API GeoDb
            var configMock = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"GeoDb:BaseUrl", "https://fake.api"},
                    {"GeoDb:ApiKey", "test"},
                    {"GeoDb:ApiHost", "host"}
                })
                .Build();

            var service = new GeoDbService(client, configMock);

            // Act
            // Wywołanie testowanej metody, która ma zwrócić listę miast na podstawie zapytania
            var result = await service.SearchCitiesAsync("Paris");

            // Assert
            Assert.Single(result);
            Assert.Equal("Paris", result[0].City);
            Assert.Equal("France", result[0].Country);
        }
    }
}
