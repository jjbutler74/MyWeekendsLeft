using System;
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
        [ApiVersion("1.0")]
        [Route("")]
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
        [ApiVersion("1.0")]
        [Route("version/")]
        public VersionInfo Version()
        {
            var ver = _weekendsLeftService.GetVersion();
            return ver;
        }

        [HttpGet]
        [ApiVersion("2.0")]
        [Route("")]
        public string WeekendsLeftResponse2()
        {
            return "Version 2.0 is not yet implemented.";
        }

        [HttpGet]
        [ApiVersion("2.0")]
        [Route("version/")]
        public string Version2()
        {
            return "Version 2.0 is not yet implemented.";
        }
    }
}
