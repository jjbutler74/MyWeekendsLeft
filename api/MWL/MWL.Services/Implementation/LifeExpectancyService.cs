using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MWL.Models;
using MWL.Models.Entities;
using MWL.Services.Interface;

namespace MWL.Services.Implementation
{
    public class LifeExpectancyService : ILifeExpectancyService
    {
        private readonly IConfiguration _config;
        private readonly ICountriesService _countriesService;
        private readonly HttpClient _httpClient;
        private readonly ILogger<LifeExpectancyService> _logger;
        private readonly IMemoryCache _cache;

        public LifeExpectancyService(IConfiguration config, ICountriesService countriesService, HttpClient httpClient, ILogger<LifeExpectancyService> logger, IMemoryCache cache)
        {
            _config = config;
            _countriesService = countriesService;
            _httpClient = httpClient;
            _logger = logger;
            _cache = cache;
        }

        public async Task<double> GetRemainingLifeExpectancyYearsAsync(WeekendsLeftRequest weekendsLeftRequest)
        {
            // Create cache key based on request parameters
            var cacheKey = $"LifeExpectancy_{weekendsLeftRequest.Age}_{weekendsLeftRequest.Gender}_{weekendsLeftRequest.Country.ToUpper()}";

            // Try to get from cache first
            if (_cache.TryGetValue(cacheKey, out double cachedValue))
            {
                _logger.LogInformation("Life expectancy retrieved from cache for {CacheKey}", cacheKey);
                return cachedValue;
            }

            try
            {
                // Build Url
                var uri = _config.GetValue<string>("MwlConfiguration:LifeExpectancyApiUri");
                var formatGender = weekendsLeftRequest.Gender.ToString().ToLower();
                var countryData = _countriesService.GetCountryData();
                var formatCountry = Uri.EscapeDataString(countryData[weekendsLeftRequest.Country.ToUpper()]);
                var formatDate = DateTime.Now.ToString("yyyy-MM-dd");
                var formatAge = weekendsLeftRequest.Age;
                var fullUrl = $"{uri}/{formatGender}/{formatCountry}/{formatDate}/{formatAge}y/";

                _logger.LogInformation("Calling life expectancy API: {Url}", fullUrl);

                var response = await _httpClient.GetStringAsync(fullUrl);

                var responseDeserialize = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(response);
                var remainingLife = (double)responseDeserialize.remaining_life_expectancy;

                // Cache the result for 24 hours
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(24));
                _cache.Set(cacheKey, remainingLife, cacheOptions);

                _logger.LogInformation("Life expectancy API returned: {RemainingLife} years (cached for 24 hours)", remainingLife);
                return remainingLife;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error calling life expectancy API for age {Age}, gender {Gender}, country {Country}",
                    weekendsLeftRequest.Age, weekendsLeftRequest.Gender, weekendsLeftRequest.Country);
                throw new InvalidOperationException("Unable to retrieve life expectancy data from external service. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing life expectancy data for age {Age}, gender {Gender}, country {Country}",
                    weekendsLeftRequest.Age, weekendsLeftRequest.Gender, weekendsLeftRequest.Country);
                throw new InvalidOperationException("An error occurred while processing life expectancy data.", ex);
            }
        }

        public WeekendsLeftResponse LifeExpectancyCalculations(int age, double remainingLifeExpectancyYears)
        {
            var weekendsLeftResponse = new WeekendsLeftResponse();

            var estimatedAgeOfDeath = (int)(age + remainingLifeExpectancyYears);
            var estimatedDaysLeft = (int)(remainingLifeExpectancyYears * 365);
            var estimatedDayOfDeath = DateTime.Now.AddDays(estimatedDaysLeft);
            var estimatedWeekendsLeft = estimatedDaysLeft / 7;
            
            weekendsLeftResponse.EstimatedAgeOfDeath = estimatedAgeOfDeath;
            weekendsLeftResponse.EstimatedDayOfDeath = estimatedDayOfDeath;
            weekendsLeftResponse.EstimatedWeekendsLeft = estimatedWeekendsLeft;
            weekendsLeftResponse.Message = $"You have an estimated {estimatedWeekendsLeft} weekends left in your life, get out there and enjoy it all";
            return weekendsLeftResponse;
        }
    }
}
