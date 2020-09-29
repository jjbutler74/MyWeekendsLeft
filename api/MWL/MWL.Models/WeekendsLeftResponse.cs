using System;
using System.Collections.Generic;

namespace MWL.Models
{
    public class WeekendsLeftResponse
    {
        public DateTime EstimatedDayOfDeath {get; set;}
        public int EstimatedAgeOfDeath {get; set;}
        public int EstimatedWeekendsLeft {get; set;}
        public string Message { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
