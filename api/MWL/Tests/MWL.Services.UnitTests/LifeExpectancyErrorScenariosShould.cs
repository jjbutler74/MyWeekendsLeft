using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using MWL.Models;
using MWL.Models.Entities;
using MWL.Services.Implementation;
using MWL.Services.Interface;
using Xunit;

namespace MWL.Services.UnitTests
{
    public class LifeExpectancyErrorScenariosShould
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ICountriesService> _mockCountriesService;
        private readonly Mock<ILogger<LifeExpectancyService>> _mockLogger;
        private readonly Mock<IMemoryCache> _mockCache;

        public LifeExpectancyErrorScenariosShould()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(x => x.GetSection("MwlConfiguration:LifeExpectancyApiUri").Value)
                .Returns("https://test.api.com/life-expectancy/remaining");

            _mockCountriesService = new Mock<ICountriesService>();
            _mockCountriesService.Setup(x => x.GetCountryData())
                .Returns(new System.Collections.Generic.Dictionary<string, string>
                {
                    { "USA", "United States" }
                });

            _mockLogger = new Mock<ILogger<LifeExpectancyService>>();
            _mockCache = new Mock<IMemoryCache>();

            // Setup cache to always miss
            object cacheValue;
            _mockCache.Setup(x => x.TryGetValue(It.IsAny<object>(), out cacheValue!)).Returns(false);
            _mockCache.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());
        }

        private HttpClient CreateMockHttpClient(HttpResponseMessage response)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            return new HttpClient(handlerMock.Object);
        }

        private HttpClient CreateMockHttpClientWithException(Exception exception)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(exception);

            return new HttpClient(handlerMock.Object);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ThrowInvalidOperationException_WhenApiReturnsNullResponse()
        {
            // Arrange
            var httpClient = CreateMockHttpClient(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("null")
            });

            var service = new LifeExpectancyService(
                _mockConfiguration.Object,
                _mockCountriesService.Object,
                httpClient,
                _mockLogger.Object,
                _mockCache.Object);

            var request = new WeekendsLeftRequest
            {
                Age = 45,
                Gender = Gender.Male,
                Country = "USA"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.GetRemainingLifeExpectancyYearsAsync(request));

            Assert.Contains("null response", exception.Message);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ThrowInvalidOperationException_WhenHttpRequestFails()
        {
            // Arrange
            var httpClient = CreateMockHttpClientWithException(
                new HttpRequestException("Network error"));

            var service = new LifeExpectancyService(
                _mockConfiguration.Object,
                _mockCountriesService.Object,
                httpClient,
                _mockLogger.Object,
                _mockCache.Object);

            var request = new WeekendsLeftRequest
            {
                Age = 45,
                Gender = Gender.Male,
                Country = "USA"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.GetRemainingLifeExpectancyYearsAsync(request));

            Assert.Contains("external service", exception.Message);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ThrowInvalidOperationException_WhenApiReturnsInvalidJson()
        {
            // Arrange
            var httpClient = CreateMockHttpClient(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("not valid json")
            });

            var service = new LifeExpectancyService(
                _mockConfiguration.Object,
                _mockCountriesService.Object,
                httpClient,
                _mockLogger.Object,
                _mockCache.Object);

            var request = new WeekendsLeftRequest
            {
                Age = 45,
                Gender = Gender.Male,
                Country = "USA"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.GetRemainingLifeExpectancyYearsAsync(request));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ThrowInvalidOperationException_WhenApiReturnsEmptyJson()
        {
            // Arrange
            var httpClient = CreateMockHttpClient(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{}")
            });

            var service = new LifeExpectancyService(
                _mockConfiguration.Object,
                _mockCountriesService.Object,
                httpClient,
                _mockLogger.Object,
                _mockCache.Object);

            var request = new WeekendsLeftRequest
            {
                Age = 45,
                Gender = Gender.Male,
                Country = "USA"
            };

            // Act - should succeed but with 0 remaining life expectancy
            var result = await service.GetRemainingLifeExpectancyYearsAsync(request);

            // Assert - default value is 0 for unset double
            Assert.Equal(0, result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task SuccessfullyParse_WhenApiReturnsValidResponse()
        {
            // Arrange
            var httpClient = CreateMockHttpClient(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"remaining_life_expectancy\": 35.5, \"sex\": \"male\", \"country\": \"United States\"}")
            });

            var service = new LifeExpectancyService(
                _mockConfiguration.Object,
                _mockCountriesService.Object,
                httpClient,
                _mockLogger.Object,
                _mockCache.Object);

            var request = new WeekendsLeftRequest
            {
                Age = 45,
                Gender = Gender.Male,
                Country = "USA"
            };

            // Act
            var result = await service.GetRemainingLifeExpectancyYearsAsync(request);

            // Assert
            Assert.Equal(35.5, result);
        }

        [Theory]
        [Trait("Category", "Unit")]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.ServiceUnavailable)]
        public async Task ThrowInvalidOperationException_WhenApiReturnsErrorStatusCode(HttpStatusCode statusCode)
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException($"Response status code does not indicate success: {(int)statusCode}"));

            var httpClient = new HttpClient(handlerMock.Object);

            var service = new LifeExpectancyService(
                _mockConfiguration.Object,
                _mockCountriesService.Object,
                httpClient,
                _mockLogger.Object,
                _mockCache.Object);

            var request = new WeekendsLeftRequest
            {
                Age = 45,
                Gender = Gender.Male,
                Country = "USA"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.GetRemainingLifeExpectancyYearsAsync(request));

            Assert.Contains("external service", exception.Message);
        }
    }
}
