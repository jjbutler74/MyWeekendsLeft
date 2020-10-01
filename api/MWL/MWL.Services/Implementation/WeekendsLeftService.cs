using MWL.Services.Interface;
using MWL.Models;
using MWL.Models.Validators;
using System;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace MWL.Services.Implementation
{
    public class WeekendsLeftService : IWeekendsLeftService
    {
        private ICountriesService _countriesService;
        private ILifeExpectancyService _lifeExpectancyService;

        public WeekendsLeftService(ICountriesService countriesService, ILifeExpectancyService lifeExpectancyService)
        {
            _countriesService = countriesService;
            _lifeExpectancyService = lifeExpectancyService;
        }

        public async Task<WeekendsLeftResponse> GetWeekendsLeftAsync(WeekendsLeftRequest weekendsLeftRequest)
        {
            var weekendsLeftResponse = new WeekendsLeftResponse();

            // Model Validation                         
            var validator = new WeekendsLeftRequestValidator();
            var results = validator.Validate(weekendsLeftRequest);
            if (!results.IsValid)
            {
                weekendsLeftResponse.Errors = results.Errors.Select(errors => errors.ErrorMessage).ToList();
                weekendsLeftResponse.Message = "Errors in request, please correct and resubmit";
                return weekendsLeftResponse;
            }

            // County Code Validation
            if (!_countriesService.GetCountryData().ContainsKey(weekendsLeftRequest.Country.ToUpper()))
            {
                weekendsLeftResponse.Errors = new[] {"Country Code is not valid"};
                weekendsLeftResponse.Message = "Errors in request, please correct and resubmit";
                return weekendsLeftResponse;
            }

            // Life Expectancy Lookup - MADE UP FOR NOW

            var jjb = await _lifeExpectancyService.GetWeekendsLifeExpectancy(weekendsLeftRequest);



            var rng = new Random();
            var estimatedDayOfDeath = DateTime.Now.AddDays(rng.Next(100, 20000));
            var estimatedDaysLeft = (estimatedDayOfDeath - DateTime.Now).Days;
            var estimatedAgeOfDeath = weekendsLeftRequest.Age + (estimatedDaysLeft / 365);
            //var estimatedWeekendsLeft = estimatedDaysLeft / 7;
            var estimatedWeekendsLeft = jjb;

            weekendsLeftResponse.EstimatedDayOfDeath = estimatedDayOfDeath;
            weekendsLeftResponse.EstimatedAgeOfDeath = estimatedAgeOfDeath;
            weekendsLeftResponse.EstimatedWeekendsLeft = estimatedWeekendsLeft;
            weekendsLeftResponse.Message = $"You have an estimated {estimatedWeekendsLeft} weekends left in your life, get out there and enjoy it!";
            
            return weekendsLeftResponse;
        }
    }
}