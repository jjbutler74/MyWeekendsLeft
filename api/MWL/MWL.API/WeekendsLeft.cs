using System;

namespace MWL.API
{
    public class WeekendsLeft
    {
        public int EstimatedWeekendsLeft { get; set; }
        public DateTime EstimatedDayOfDeath { get; set; }
        public int EstimatedAgeOfDeath { get; set; }

        public string Summary { get; set; }
    }
}
