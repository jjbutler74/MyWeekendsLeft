using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MWL.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeekendsLeftController : ControllerBase
    {
        private readonly ILogger<WeekendsLeftController> _logger;

        public WeekendsLeftController(ILogger<WeekendsLeftController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public WeekendsLeft Get([FromQuery] int age)
        {
            var rng = new Random();
            var estimatedDayOfDeath = DateTime.Now.AddDays(rng.Next(100, 20000));
            var estimatedDaysLeft = (estimatedDayOfDeath - DateTime.Now).Days;
            var estimatedAgeOfDeath = age + (estimatedDaysLeft / 365);
            var estimatedWeekendsLeft = estimatedDaysLeft / 7;

            var weekendsLeft = new WeekendsLeft
            {
                EstimatedDayOfDeath = estimatedDayOfDeath,
                EstimatedAgeOfDeath = estimatedAgeOfDeath,
                EstimatedWeekendsLeft = estimatedWeekendsLeft,
                Summary = $"You have an estiamted {estimatedWeekendsLeft} weekends left in your life, enjoy!"
            };

            return weekendsLeft;
        }
    }
}
