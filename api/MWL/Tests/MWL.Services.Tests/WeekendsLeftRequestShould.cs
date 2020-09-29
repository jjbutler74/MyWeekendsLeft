using MWL.Services.Implementation;
using MWL.Models;
using Xunit;

namespace MWL.Services.Tests
{
    public class WeekendsLeftRequestShould 
    {
        static readonly int MAXAGE = 120;

        [Fact]
        public void HaveEstimatedAgeOfDeathInRange()
        {
            // Arrange
            var weekendsLeftService = new WeekendsLeftService(); // Not using DI on this now

            var weekendsLeftRequest = new WeekendsLeftRequest
            {
                Age = 45
            };

            // Act
            var weekendsLeftResponse = weekendsLeftService.GetWeekendsLeft(weekendsLeftRequest);

            // Assert
            Assert.InRange(weekendsLeftResponse.EstimatedAgeOfDeath, weekendsLeftRequest.Age, MAXAGE);
        }

        [Fact]
        public void NotAllowNegativeAges()
        {
            // Arrange
            var weekendsLeftService = new WeekendsLeftService();

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
            var weekendsLeftService = new WeekendsLeftService();

            var weekendsLeftRequest = new WeekendsLeftRequest
            {
                Age = 45
            };

            // Act
            var weekendsLeftResponse = weekendsLeftService.GetWeekendsLeft(weekendsLeftRequest);

            // Assert
            Assert.StartsWith("You have an estimated", weekendsLeftResponse.Message);
            Assert.EndsWith("weekends left in your life, get out there and enjoy it!", weekendsLeftResponse.Message);
        }
    }
}
