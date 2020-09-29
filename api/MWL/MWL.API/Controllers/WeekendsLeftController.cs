using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MWL.Services.Interface;
using MWL.Models;

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
        public WeekendsLeftResponse Get([FromQuery] int age)
        {
            var weekendsLeftRequest = new WeekendsLeftRequest
            {
                Age = age
            };

            var weekendsLeftResponse = _weekendsLeftService.GetWeekendsLeft(weekendsLeftRequest);
            return weekendsLeftResponse;
        }
    }
}
