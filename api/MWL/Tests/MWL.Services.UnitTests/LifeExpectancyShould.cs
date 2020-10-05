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
    }
}