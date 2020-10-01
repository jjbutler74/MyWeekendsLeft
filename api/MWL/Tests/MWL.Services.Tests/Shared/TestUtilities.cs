using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace MWL.Services.Tests
{
    public class TestUtilities
    {
        public static IConfigurationRoot ConfigurationRoot()
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
    }
}
