using MWL.Domain.Implementation;
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
            Assert.Contains("Cannot", weekendsLeftResponse.Summary);
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
            Assert.StartsWith("You have an estiamted", weekendsLeftResponse.Summary);
            Assert.EndsWith("weekends left in your life, get out there and enjoy it!", weekendsLeftResponse.Summary);
        }
    }
}
