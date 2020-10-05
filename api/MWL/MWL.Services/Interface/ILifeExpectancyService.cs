using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MWL.Models;

namespace MWL.Services.Interface
{
    public interface ILifeExpectancyService
    {
        Task<double> GetRemainingLifeExpectancyYearsAsync(WeekendsLeftRequest weekendsLeftRequest);
        WeekendsLeftResponse LifeExpectancyCalculations(int age, double remainingLifeExpectancyYears);
    }
}
