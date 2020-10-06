using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MWL.Services.Interface;
using MWL.Models;
using MWL.Models.Entities;

namespace MWL.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeekendsLeftController : ControllerBase
    {
        private readonly ILogger<WeekendsLeftController> _logger;
        private readonly IWeekendsLeftService _weekendsLeftService;

        public WeekendsLeftController(ILogger<WeekendsLeftController> logger, IWeekendsLeftService weekendsLeftService)
        {
            _logger = logger;
            _weekendsLeftService = weekendsLeftService;
        }
        
        [HttpGet]
        public async Task<WeekendsLeftResponse> GetAsync([FromQuery] int age, string gender, string country)
        {
            Enum.TryParse(gender,true, out Gender gen);
            var weekendsLeftRequest = new WeekendsLeftRequest
            {
                Age = age,
                Gender = gen,
                Country = country
            };

            var weekendsLeftResponse = await _weekendsLeftService.GetWeekendsLeftAsync(weekendsLeftRequest);
            return weekendsLeftResponse;
        }

        [HttpGet]
        [Route("version/")]
        public string Version()
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

            return $"Version: {version} v2: {ver2} rel: {releaseName} bld: {buildNumber}";
        }
    }
}
