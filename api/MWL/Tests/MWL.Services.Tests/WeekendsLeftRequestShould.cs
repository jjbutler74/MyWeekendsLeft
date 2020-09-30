using Microsoft.Extensions.Caching.Memory;
using MWL.Services.Implementation;
using MWL.Models;
using MWL.Models.Entities;
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

        [Fact]
        public void HaveEstimatedAgeOfDeathInRange()
        {
            // Arrange
            var cache = new MemoryCache(new MemoryCacheOptions());
            var countriesService = new CountriesService(cache);
            var weekendsLeftService = new WeekendsLeftService(countriesService);

            var weekendsLeftRequest = new WeekendsLeftRequest
            {
                Age = 45,
                Gender = Gender.Female,
                Country = "USA"
            };

            // Act
            var weekendsLeftResponse = weekendsLeftService.GetWeekendsLeft(weekendsLeftRequest);

            // Assert
            Assert.InRange(weekendsLeftResponse.EstimatedAgeOfDeath, weekendsLeftRequest.Age, MAXAGE);
            _output.WriteLine("HaveEstimatedAgeOfDeathInRange was tested");
        }

        [Fact]
        public void NotAllowNegativeAges()
        {
            // Arrange
            var cache = new MemoryCache(new MemoryCacheOptions());
            var countriesService = new CountriesService(cache);
            var weekendsLeftService = new WeekendsLeftService(countriesService);

            var weekendsLeftRequest = new WeekendsLeftRequest
            {
                Age = -5
            };

            // Act
            var weekendsLeftResponse = weekendsLeftService.GetWeekendsLeft(weekendsLeftRequest);

            // Assert
            Assert.Contains("'Age' must be between 1 and 120. You entered -5.", weekendsLeftResponse.Errors);
        }

        [Fact]
        public void HaveCorrectSummaryText()
        {
            // Arrange
            var cache = new MemoryCache(new MemoryCacheOptions());
            var countriesService = new CountriesService(cache);
            var weekendsLeftService = new WeekendsLeftService(countriesService);

            var weekendsLeftRequest = new WeekendsLeftRequest
            {
                Age = 55,
                Gender = Gender.Male,
                Country = "NZL"
            };

            // Act
            var weekendsLeftResponse = weekendsLeftService.GetWeekendsLeft(weekendsLeftRequest);

            // Assert
            Assert.StartsWith("You have an estimated", weekendsLeftResponse.Message);
            Assert.EndsWith("weekends left in your life, get out there and enjoy it!", weekendsLeftResponse.Message);
        }
    }
}
