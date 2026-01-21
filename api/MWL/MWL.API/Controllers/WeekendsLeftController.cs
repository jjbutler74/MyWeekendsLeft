using System;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MWL.Services.Interface;
using MWL.Models;
using MWL.Models.Entities;

namespace MWL.API.Controllers
{
    [ApiController]
    [Route("api")]
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
        [Route("getweekends/")]
        [ProducesResponseType(typeof(WeekendsLeftResponse), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 503)]
        public async Task<ActionResult<WeekendsLeftResponse>> GetAsync([FromQuery] int age, string gender, string country)
        {
            _logger.LogInformation("Received request for age: {Age}, gender: {Gender}, country: {Country}", age, gender, country);

            // Validate gender parameter
            if (!Enum.TryParse<Gender>(gender, true, out Gender gen) || gen == Gender.Unknown)
            {
                _logger.LogWarning("Invalid gender parameter: {Gender}", gender);
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid Gender",
                    Detail = $"Gender '{gender}' is not valid. Allowed values: Male, Female",
                    Status = 400
                });
            }

            var weekendsLeftRequest = new WeekendsLeftRequest
            {
                Age = age,
                Gender = gen,
                Country = country
            };

            try
            {
                var weekendsLeftResponse = await _weekendsLeftService.GetWeekendsLeftAsync(weekendsLeftRequest);

                // Check if validation errors occurred
                if (weekendsLeftResponse.Errors != null && weekendsLeftResponse.Errors.Any())
                {
                    _logger.LogWarning("Validation failed for request: {Errors}", string.Join(", ", weekendsLeftResponse.Errors));
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Validation Failed",
                        Detail = string.Join("; ", weekendsLeftResponse.Errors),
                        Status = 400
                    });
                }

                _logger.LogInformation("Successfully calculated weekends left: {Weekends}", weekendsLeftResponse.EstimatedWeekendsLeft);
                return Ok(weekendsLeftResponse);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Service unavailable while processing request");
                return StatusCode(503, new ProblemDetails
                {
                    Title = "Service Unavailable",
                    Detail = ex.Message,
                    Status = 503
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error processing weekends left request");
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Internal Server Error",
                    Detail = "An unexpected error occurred while processing your request",
                    Status = 500
                });
            }
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
        [Route("getweekends/")]
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
