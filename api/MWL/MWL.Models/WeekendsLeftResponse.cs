using System;
using System.Collections.Generic;
using System.Text;

namespace MWL.Models
{
    public class WeekendsLeftResponse
    {
        public DateTime EstimatedDayOfDeath {get; set;}
        public int EstimatedAgeOfDeath {get; set;}
        public int EstimatedWeekendsLeft {get; set;}
        public string Summary { get; set; }
    }
}
