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
            var ver = _weekendsLeftService.GetVersion();
            return ver;
        }
    }
}
