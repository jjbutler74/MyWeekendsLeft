using MWL.Services.Interface;
using MWL.Models;
using MWL.Models.Validators;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MWL.Models.Entities;

namespace MWL.Services.Implementation
{
    public class WeekendsLeftService : IWeekendsLeftService
    {
        private readonly IConfiguration _config;
        private ICountriesService _countriesService;
        private ILifeExpectancyService _lifeExpectancyService;

        public WeekendsLeftService(IConfiguration config, ICountriesService countriesService, ILifeExpectancyService lifeExpectancyService)
        {
            _config = config;
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

            // Country Code Validation
            if (!_countriesService.GetCountryData().ContainsKey(weekendsLeftRequest.Country.ToUpper()))
            {
                weekendsLeftResponse.Errors = new[] {"Country Code is not valid"};
                weekendsLeftResponse.Message = "Errors in request, please correct and resubmit";
                return weekendsLeftResponse;
            }

            // Life Expectancy Lookup
            var remainingLifeExpectancyYears = await _lifeExpectancyService.GetRemainingLifeExpectancyYearsAsync(weekendsLeftRequest);

            // Life Expectancy Calculations
            weekendsLeftResponse = _lifeExpectancyService.LifeExpectancyCalculations(weekendsLeftRequest.Age, remainingLifeExpectancyYears);

            return weekendsLeftResponse;
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