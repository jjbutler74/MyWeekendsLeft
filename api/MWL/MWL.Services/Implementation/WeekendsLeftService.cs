using MWL.Services.Interface;
using MWL.Models;
using MWL.Models.Validators;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MWL.Models.Entities;

namespace MWL.Services.Implementation
{
    public class WeekendsLeftService : IWeekendsLeftService
    {
        private readonly IConfiguration _config;
        private readonly ICountriesService _countriesService;
        private readonly ILifeExpectancyService _lifeExpectancyService;
        private readonly ILogger<WeekendsLeftService> _logger;

        public WeekendsLeftService(IConfiguration config, ICountriesService countriesService, ILifeExpectancyService lifeExpectancyService, ILogger<WeekendsLeftService> logger)
        {
            _config = config;
            _countriesService = countriesService;
            _lifeExpectancyService = lifeExpectancyService;
            _logger = logger;
        }

        public async Task<WeekendsLeftResponse> GetWeekendsLeftAsync(WeekendsLeftRequest weekendsLeftRequest)
        {
            _logger.LogInformation("Processing weekends left request for age {Age}, gender {Gender}, country {Country}",
                weekendsLeftRequest.Age, weekendsLeftRequest.Gender, weekendsLeftRequest.Country);

            var weekendsLeftResponse = new WeekendsLeftResponse();

            // Model Validation
            var validator = new WeekendsLeftRequestValidator();
            var results = validator.Validate(weekendsLeftRequest);
            if (!results.IsValid)
            {
                _logger.LogWarning("Validation failed for request: {Errors}",
                    string.Join(", ", results.Errors.Select(e => e.ErrorMessage)));
                weekendsLeftResponse.Errors = results.Errors.Select(errors => errors.ErrorMessage).ToList();
                weekendsLeftResponse.Message = "Errors in request, please correct and resubmit";
                return weekendsLeftResponse;
            }

            // Country Code Validation
            if (!_countriesService.GetCountryData().ContainsKey(weekendsLeftRequest.Country.ToUpper()))
            {
                _logger.LogWarning("Invalid country code: {Country}", weekendsLeftRequest.Country);
                weekendsLeftResponse.Errors = new[] {"Country Code is not valid"};
                weekendsLeftResponse.Message = "Errors in request, please correct and resubmit";
                return weekendsLeftResponse;
            }

            try
            {
                // Life Expectancy Lookup
                var remainingLifeExpectancyYears = await _lifeExpectancyService.GetRemainingLifeExpectancyYearsAsync(weekendsLeftRequest);

                // Life Expectancy Calculations
                weekendsLeftResponse = _lifeExpectancyService.LifeExpectancyCalculations(weekendsLeftRequest.Age, remainingLifeExpectancyYears);

                _logger.LogInformation("Successfully calculated {Weekends} weekends left for age {Age}",
                    weekendsLeftResponse.EstimatedWeekendsLeft, weekendsLeftRequest.Age);

                return weekendsLeftResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating weekends left for age {Age}, gender {Gender}, country {Country}",
                    weekendsLeftRequest.Age, weekendsLeftRequest.Gender, weekendsLeftRequest.Country);
                throw;
            }
        }

        VersionInfo IWeekendsLeftService.GetVersion()
        {
            var buildNumber = _config.GetValue<string>("BuildNumber");
            var env = _config.GetValue<string>("Environment");
            var runtime = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
            var zone = $"{DateTime.Now} {TimeZoneInfo.Local.DisplayName}";

            var ver = new VersionInfo
            {
                Build = buildNumber,
                Environment = env,
                Runtime = runtime,
                ServerDatetime = zone
            };
            return ver;
        }
    }
}