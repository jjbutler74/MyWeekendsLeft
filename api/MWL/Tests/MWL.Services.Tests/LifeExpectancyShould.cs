using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Moq;
using MWL.Models;
using MWL.Models.Entities;
using MWL.Services.Implementation;
using MWL.Services.Interface;
using Xunit;

namespace MWL.Services.Tests
{
    public class LifeExpectancyShould
    {
        public LifeExpectancyService lifeExpectancyService;

        public LifeExpectancyShould()
        {
            var mockConfiguration = new Mock<IConfiguration>(); // Moq
            var mockCountriesService = new Mock<ICountriesService>(); // Moq
            lifeExpectancyService = new LifeExpectancyService(mockConfiguration.Object, mockCountriesService.Object); // Moq
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void HaveEstimatedAgeOfDeath65()
        {
            // Arrange - done in constructor 

            // Act
            var weekendsLeftResponse = lifeExpectancyService.LifeExpectancyCalculations(45, 20);

            // Assert
            Assert.Equal(65, weekendsLeftResponse.EstimatedAgeOfDeath);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void HaveEstimatedEstimatedWeekendsLeft1512()
        {
            // Arrange - done in constructor 

            // Act
            var weekendsLeftResponse = lifeExpectancyService.LifeExpectancyCalculations(45, 29);

            // Assert
            Assert.Equal(1512, weekendsLeftResponse.EstimatedWeekendsLeft);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void HaveTheCorrectMessage()
        {
            // Arrange - done in constructor 

            // Act
            var weekendsLeftResponse = lifeExpectancyService.LifeExpectancyCalculations(45, 30);

            // Assert
            Assert.Equal("You have an estimated 1564 weekends left in your life, get out there and enjoy it!", weekendsLeftResponse.Message);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void HaveRemainingLifeExpectancyYearsInRange()
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
            var remainingLifeExpectancyYears = lifeExpectancyServiceIntegration.GetRemainingLifeExpectancyYearsAsync(wlr).Result;

            // Assert
            Assert.InRange(remainingLifeExpectancyYears, 2, 10); // 7.77 on 5/10/2020
        }
    }
}