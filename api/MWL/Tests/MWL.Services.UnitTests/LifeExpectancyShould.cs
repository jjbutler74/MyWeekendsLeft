using Microsoft.Extensions.Configuration;
using Moq;
using MWL.Services.Implementation;
using MWL.Services.Interface;
using Xunit;

namespace MWL.Services.UnitTests
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

        [Theory]
        [Trait("Category", "Unit")]
        [InlineData(45, 20, 65)]
        [InlineData(45, 30, 75)]
        [InlineData(33, 33, 66)]
        [InlineData(80, 0, 80)]
        public void HaveEstimatedAgeOfDeath(int age, double remainingLifeExpectancyYears, int estimatedAgeOfDeath)
        {
            // Arrange - done in constructor 

            // Act
            var weekendsLeftResponse = lifeExpectancyService.LifeExpectancyCalculations(age, remainingLifeExpectancyYears);

            // Assert
            Assert.Equal(estimatedAgeOfDeath, weekendsLeftResponse.EstimatedAgeOfDeath);
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
            Assert.Equal("You have an estimated 1564 weekends left in your life, get out there and enjoy it all!!", weekendsLeftResponse.Message);
        }
    }
}