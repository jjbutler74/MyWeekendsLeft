using Microsoft.Extensions.Caching.Memory;
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
            var countriesService = new CountriesService(cache);
            var configuration = TestUtilities.ConfigurationRoot();
            var lifeExpectancyServiceIntegration =  new LifeExpectancyService(configuration, countriesService);

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