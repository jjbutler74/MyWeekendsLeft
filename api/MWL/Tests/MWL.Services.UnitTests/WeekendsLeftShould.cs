using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using MWL.Models;
using MWL.Models.Entities;
using MWL.Services.Implementation;
using MWL.Services.Interface;
using Xunit;
using Xunit.Abstractions;

namespace MWL.Services.UnitTests
{
    public class WeekendsLeftShould
    {
        private WeekendsLeftService weekendsLeftService;
        private readonly ITestOutputHelper _output;
        static readonly int MAXAGE = 120;

        public WeekendsLeftShould(ITestOutputHelper output)
        {
            var mockConfiguration = new Mock<IConfiguration>(); // Moq
            var mockCountriesService = new Mock<ICountriesService>(); // Moq
            var mockDict = new Dictionary<string, string>()
            {
                {"USA","United States"},
                {"NZL", "New Zealand"}
            };
            mockCountriesService.Setup(x => x.GetCountryData()).Returns(mockDict);
            
            var mockLifeExpectancyService = new Mock<ILifeExpectancyService>(); // Moq
            var wlr = new WeekendsLeftResponse
            {
                EstimatedWeekendsLeft = 1623,
                EstimatedAgeOfDeath = 86,
                EstimatedDayOfDeath = new DateTime(2051, 11, 13, 02, 48, 17),
                Message = "You have an estimated 1623 weekends left in your life, get out there and enjoy it!",
                Errors = null
            };
            mockLifeExpectancyService.Setup(x => x.LifeExpectancyCalculations(Moq.It.IsAny<int>(),Moq.It.IsAny<double>())).Returns(wlr);
            mockLifeExpectancyService.Setup(x => x.GetRemainingLifeExpectancyYearsAsync(It.IsAny<WeekendsLeftRequest>())).ReturnsAsync(34.8);

            var mockLogger = new Mock<ILogger<WeekendsLeftService>>();
            weekendsLeftService = new WeekendsLeftService(mockConfiguration.Object, mockCountriesService.Object, mockLifeExpectancyService.Object, mockLogger.Object);

            _output = output;
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task HaveEstimatedAgeOfDeathInRangeAsync()
        {
            // Arrange - done in constructor 

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

        [Theory]
        [Trait("Category", "Unit")]
        [InlineData(-5)]
        [InlineData(0)]
        [InlineData(121)]
        public async Task NotAllowNegativeAgesAsync(int age)
        {
            // Arrange - done in constructor

            var weekendsLeftRequest = new WeekendsLeftRequest
            {
                Age = age
            };

            // Act
            var weekendsLeftResponse = await weekendsLeftService.GetWeekendsLeftAsync(weekendsLeftRequest);

            // Assert
            Assert.Contains($"'Age' must be between 1 and 120. You entered {age}.", weekendsLeftResponse.Errors);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task HaveCorrectSummaryTextAsync()
        {
            // Arrange - done in constructor 

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
