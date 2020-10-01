using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MWL.Models;

namespace MWL.Services.Interface
{
    public interface ILifeExpectancyService
    {
        Task<int> GetWeekendsLifeExpectancy(WeekendsLeftRequest weekendsLeftRequest);
    }
}
