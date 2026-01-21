using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using MWL.Services.Implementation;
using Xunit;

namespace MWL.Services.IntegrationTests
{
    public class CountiesShould
    {
        // Tests depends on IO to embedded file - JS says it's ok
        // https://stackoverflow.com/questions/52186343/is-reading-csv-file-from-physical-path-is-valid-scenario-in-unit-test-case-why/52186784

        [Fact]
        [Trait("Category", "Integration")]
        public void ContainNewZealand()
        {
            // Arrange
            var cache = new MemoryCache(new MemoryCacheOptions()); // https://stackoverflow.com/a/40685073/5854
            var mockLogger = new Mock<ILogger<CountriesService>>();
            var countriesService = new CountriesService(cache, mockLogger.Object);

            // Act
            var countries = countriesService.GetCountryData();

            // Assert
            Assert.Contains(countries, c => c.Value.Contains("New Zealand") );
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void HaveReasonableCount()
        {
            // Arrange
            var cache = new MemoryCache(new MemoryCacheOptions());
            var mockLogger = new Mock<ILogger<CountriesService>>();
            var countriesService = new CountriesService(cache, mockLogger.Object);

            // Act
            var countries = countriesService.GetCountryData();

            // Assert
            Assert.InRange(countries.Count, 180, 220);
        }
    }
}
