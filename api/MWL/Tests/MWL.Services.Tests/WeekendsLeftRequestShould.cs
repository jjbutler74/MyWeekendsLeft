using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using MWL.Services.Implementation;
using MWL.Models;
using MWL.Models.Entities;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Xunit;
using Xunit.Abstractions;

namespace MWL.Services.Tests
{
    public class WeekendsLeftRequestShould
    {
        private readonly ITestOutputHelper _output;
        static readonly int MAXAGE = 120;

        public WeekendsLeftRequestShould(ITestOutputHelper output)
        {
            _output = output;
        }

        private static IConfigurationRoot ConfigurationRoot()
        {
            var appConfig = new Dictionary<string, string>
            {
                {"MwlConfiguration:LifeExpectancyApiUri", "https://d6wn6bmjj722w.population.io/1.0/life-expectancy/remaining"}
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(appConfig)
                .Build();
            return configuration;
        }

        [Fact]
        public async Task HaveEstimatedAgeOfDeathInRangeAsync()
        {
            // Arrange
            var cache = new MemoryCache(new MemoryCacheOptions());
            var countriesService = new CountriesService(cache);
            var configuration = ConfigurationRoot();
            var lifeExpectancyService = new LifeExpectancyService(configuration, countriesService);
            var weekendsLeftService = new WeekendsLeftService(countriesService, lifeExpectancyService);

            var weekendsLeftRequest = new WeekendsLeftRequest
            {
                Age = 45,
                Gender = Gender.Female,
                Country = "USA"
            };

            // Act
            var weekendsLeftResponse = await weekendsLeftService.GetWeekendsLeftAsync(weekendsLeftRequest);

            // Assert
            Assert.InRange(weekendsLeftResponse.EstimatedAgeOfDeath, weekendsLeftRequest.Age, MAXAGE);
            _output.WriteLine("HaveEstimatedAgeOfDeathInRange was tested");
        }

        [Fact]
        public async Task NotAllowNegativeAgesAsync()
        {
            // Arrange
            var cache = new MemoryCache(new MemoryCacheOptions());
            var countriesService = new CountriesService(cache);
            var configuration = ConfigurationRoot();
            var lifeExpectancyService = new LifeExpectancyService(configuration, countriesService);
            var weekendsLeftService = new WeekendsLeftService(countriesService, lifeExpectancyService);

            var weekendsLeftRequest = new WeekendsLeftRequest
            {
                Age = -5
            };

            // Act
            var weekendsLeftResponse = await weekendsLeftService.GetWeekendsLeftAsync(weekendsLeftRequest);

            // Assert
            Assert.Contains("'Age' must be between 1 and 120. You entered -5.", weekendsLeftResponse.Errors);
        }

        [Fact]
        public async Task HaveCorrectSummaryTextAsync()
        {
            // Arrange
            var cache = new MemoryCache(new MemoryCacheOptions());
            var countriesService = new CountriesService(cache);
            var configuration = ConfigurationRoot();
            var lifeExpectancyService = new LifeExpectancyService(configuration, countriesService);
            var weekendsLeftService = new WeekendsLeftService(countriesService, lifeExpectancyService);

            var weekendsLeftRequest = new WeekendsLeftRequest
            {
                Age = 55,
                Gender = Gender.Male,
                Country = "NZL"
            };

            // Act
            var weekendsLeftResponse = await weekendsLeftService.GetWeekendsLeftAsync(weekendsLeftRequest);

            // Assert
            Assert.StartsWith("You have an estimated", weekendsLeftResponse.Message);
            Assert.EndsWith("weekends left in your life, get out there and enjoy it!", weekendsLeftResponse.Message);
        }
    }
}
