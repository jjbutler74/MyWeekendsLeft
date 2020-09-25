using MWL.Domain.Interface;
using MWL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;

namespace MWL.Domain.Implementation
{
    public class WeekendsLeftService : IWeekendsLeftService
    {
        public WeekendsLeftResponse GetWeekendsLeft(WeekendsLeftRequest weekendsLeftRequest)
        {
            var rng = new Random();
            var estimatedDayOfDeath = DateTime.Now.AddDays(rng.Next(100, 20000));
            var estimatedDaysLeft = (estimatedDayOfDeath - DateTime.Now).Days;
            var estimatedAgeOfDeath = weekendsLeftRequest.Age + (estimatedDaysLeft / 365);
            var estimatedWeekendsLeft = estimatedDaysLeft / 7;

            var weekendsLeftResponse = new WeekendsLeftResponse
            {
                EstimatedDayOfDeath = estimatedDayOfDeath,
                EstimatedAgeOfDeath = estimatedAgeOfDeath,
                EstimatedWeekendsLeft = estimatedWeekendsLeft,
                Summary = $"You have an estiamted {estimatedWeekendsLeft} weekends left in your life, get out there and enjoy it!"
            };
            return weekendsLeftResponse;
        }
    }
}
