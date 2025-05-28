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
    public class CountryServiceTests
    {
        /// <summary>
        /// Test potwierdza, że jeśli zewnętrzne API działa poprawnie i zwraca oczekiwany JSON,
        /// to nasz serwis potrafi go poprawnie sparsować i zwrócić listę miast.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetCapitalCitiesEuropeAsync_Returns_List()
        {
            // --- Arrange ---

            // Przykładowa odpowiedź JSON, jaką mogłoby zwrócić API
            var jsonResponse = """
            [
                {
                "capital": ["Warsaw"],
                "name": {"common": "Poland"},
                "flags": {"png": "https://flagcdn.com/w40/pl.png"},
                "cca2": "PL"
                }
            ]
            """;

            // Mockowanie handlera HTTP – udaje prawdziwe wywołanie API, ale zwraca gotową odpowiedź
            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    // podstawiony JSON jako treść odpowiedzi
                    Content = new StringContent(jsonResponse)
                });

            // Tworzenie klienta HTTP z mockowanym handlerem (nie robi prawdziwego requestu)
            var client = new HttpClient(handlerMock.Object);

            // Mockowana konfiguracja aplikacji – zawiera bazowy URL do API
            var configMock = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"RestCountries:BaseUrl", "https://restcountries.com/v3.1"}
                })
                .Build();

            // Inicjalizacja serwisu z podstawionym klientem i konfiguracją
            var service = new CountryService(client, configMock);

            // --- Act ---

            // Wywołanie testowanej metody
            var result = await service.GetCapitalCitiesEuropeAsync();

            // --- Assert ---
            // Sprawdzenie, czy zwrócono dokładnie jeden wynik
            Assert.Single(result);
            // Sprawdzenie, czy stolica to "Warsaw"
            Assert.Equal("Warsaw", result[0].Capital);
            // Sprawdzenie, czy nazwa kraju to "Poland"
            Assert.Equal("Poland", result[0].Country);
        }
    }
}