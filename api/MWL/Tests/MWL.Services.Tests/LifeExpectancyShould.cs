using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Moq;
using MWL.Models.Entities;
using MWL.Services.Implementation;
using MWL.Services.Interface;
using Xunit;

namespace MWL.Services.Tests
{
    public class LifeExpectancyShould
    {
        [Fact]
        public void HaveEstimatedAgeOfDeath65()
        {
            // Arrange
            //var cache = new MemoryCache(new MemoryCacheOptions());
            //var countriesService = new CountriesService(cache);
            //var configuration = TestUtilities.ConfigurationRoot();
            //var lifeExpectancyService = new LifeExpectancyService(configuration, countriesService);

            var mockConfiguration = new Mock<IConfiguration>(); // Moq
            var mockCountriesService = new Mock<ICountriesService>(); // Moq
            var lifeExpectancyService = new LifeExpectancyService(mockConfiguration.Object, mockCountriesService.Object); // Moq

            // Act
            var weekendsLeftResponse = lifeExpectancyService.LifeExpectancyCalculations(45, 20);

            // Assert
            Assert.Equal(65, weekendsLeftResponse.EstimatedAgeOfDeath);
        }

        [Fact]
        public void HaveEstimatedEstimatedWeekendsLeft1512()
        {
            // Arrange
            //var cache = new MemoryCache(new MemoryCacheOptions());
            //var countriesService = new CountriesService(cache);
            //var configuration = TestUtilities.ConfigurationRoot();
            //var lifeExpectancyService = new LifeExpectancyService(configuration, countriesService);

            var mockConfiguration = new Mock<IConfiguration>(); // Moq
            var mockCountriesService = new Mock<ICountriesService>(); // Moq
            var lifeExpectancyService = new LifeExpectancyService(mockConfiguration.Object, mockCountriesService.Object); // Moq

            // Act
            var weekendsLeftResponse = lifeExpectancyService.LifeExpectancyCalculations(45, 29);

            // Assert
            Assert.Equal(1512, weekendsLeftResponse.EstimatedWeekendsLeft);
        }

        [Fact]
        public void HaveTheCorrectMessage()
        {
            // Arrange
            //var cache = new MemoryCache(new MemoryCacheOptions());
            //var countriesService = new CountriesService(cache);
            //var configuration = TestUtilities.ConfigurationRoot();
            //var lifeExpectancyService = new LifeExpectancyService(configuration, countriesService);

            var mockConfiguration = new Mock<IConfiguration>(); // Moq
            var mockCountriesService = new Mock<ICountriesService>(); // Moq
            var lifeExpectancyService = new LifeExpectancyService(mockConfiguration.Object, mockCountriesService.Object); // Moq

            // Act
            var weekendsLeftResponse = lifeExpectancyService.LifeExpectancyCalculations(45, 30);

            // Assert
            Assert.Equal("You have an estimated 1564 weekends left in your life, get out there and enjoy it!", weekendsLeftResponse.Message);
        }
    }
}