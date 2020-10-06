using MWL.Services.Interface;
using MWL.Models;
using MWL.Models.Validators;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

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

            // County Code Validation
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

        string IWeekendsLeftService.GetVersion()
        {

            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.ProductVersion;

            string ver2 = "?";
            var version1 = Assembly.GetExecutingAssembly().GetName().Version;
            if (version1 is { })
            {
                ver2 = version1.ToString();
            }

            var releaseName = Environment.GetEnvironmentVariable("Release_ReleaseName", EnvironmentVariableTarget.Process);
            var buildNumber = Environment.GetEnvironmentVariable("Build_BuildNumber", EnvironmentVariableTarget.Process);

            var x = _config.GetValue<string>("BuildNumber");
            var y = _config.GetValue<string>("jjb");

            return $"Version: {version} v2: {ver2} rel: {releaseName} bld: {buildNumber} x: {x} y: {y}";


        }
    }
}