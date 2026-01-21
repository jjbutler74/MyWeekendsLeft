using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using MWL.Models;
using MWL.Models.Entities;
using MWL.Services.Implementation;
using MWL.Services.IntegrationTests.Shared;
using System.Threading.Tasks;
using Xunit;

namespace MWL.Services.IntegrationTests
{
    public class LifeExpectancyShould
    {
        [Fact]
        [Trait("Category", "Integration")]
        public async Task HaveRemainingLifeExpectancyYearsInRange()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            var mockCountriesLogger = new Mock<ILogger<CountriesService>>();
            var countriesService = new CountriesService(cache, mockCountriesLogger.Object);
            var configuration = TestUtilities.ConfigurationRoot();
            var httpClient = new HttpClient();
            var mockLifeExpectancyLogger = new Mock<ILogger<LifeExpectancyService>>();
            var lifeExpectancyServiceIntegration = new LifeExpectancyService(configuration, countriesService, httpClient, mockLifeExpectancyLogger.Object, cache);

            var wlr = new WeekendsLeftRequest
            {
                Age = 82,
                Gender = Gender.Male,
                Country = "USA"
            };

            // Act
            var remainingLifeExpectancyYears = await lifeExpectancyServiceIntegration.GetRemainingLifeExpectancyYearsAsync(wlr); //.Result;

            // Assert
            Assert.InRange(remainingLifeExpectancyYears, 2, 10); // 7.77 on 5/10/2020
        }
    }
}