using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using MWL.Models.Entities;
using MWL.Services.Implementation;
using Xunit;

namespace MWL.Services.Tests
{
    public class CountiesShould
    {
        [Fact]
        public void ContainNewZealand()
        {
            // Arrange
            var cache = new MemoryCache(new MemoryCacheOptions()); // https://stackoverflow.com/a/40685073/5854
            var countriesService = new CountriesService(cache);
            
            // Act
            var countries = countriesService.GetCountryData();

            // Assert
            Assert.Contains(countries, c => c.Value.Contains("New Zealand") );
        }

        [Fact]
        public void HaveReasonableCount()
        {
            // Arrange
            var cache = new MemoryCache(new MemoryCacheOptions());
            var countriesService = new CountriesService(cache); 

            // Act
            var countries = countriesService.GetCountryData();

            // Assert
            Assert.InRange(countries.Count, 180, 220);
        }
    }
}
