using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MWL.Models;
using MWL.Models.Entities;
using MWL.Services.Interface;

namespace MWL.Services.Implementation
{
    public class LifeExpectancyService : ILifeExpectancyService
    {
        private readonly IConfiguration _config;
        private readonly ICountriesService _countriesService;
        private static readonly HttpClient client = new HttpClient();

        public LifeExpectancyService(IConfiguration config, ICountriesService countriesService)
        {
            _config = config;
            _countriesService = countriesService;
        }

        public async Task<double> GetRemainingLifeExpectancyYearsAsync(WeekendsLeftRequest weekendsLeftRequest)
        {
            // Build Url
            var uri = _config.GetValue<string>("MwlConfiguration:LifeExpectancyApiUri");
            var formatGender = weekendsLeftRequest.Gender.ToString().ToLower();
            var countryData = _countriesService.GetCountryData();
            var formatCountry = Uri.EscapeDataString(countryData[weekendsLeftRequest.Country.ToUpper()]); 
            var formatDate = DateTime.Now.ToString("yyyy-MM-dd"); 
            var formatAge = weekendsLeftRequest.Age;
            var fullUrl = $"{uri}/{formatGender}/{formatCountry}/{formatDate}/{formatAge}y/";

            var response = await client.GetStringAsync(fullUrl);
            
            var responseDeserialize = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(response);
            return responseDeserialize.remaining_life_expectancy;
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
            weekendsLeftResponse.Message = $"You have an estimated {estimatedWeekendsLeft} weekends left in your life, get out there and enjoy it!";
            return weekendsLeftResponse;
        }
    }
}
