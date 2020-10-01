using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MWL.Models;
using MWL.Services.Interface;

namespace MWL.Services.Implementation
{
    public class LifeExpectancyService : ILifeExpectancyService
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task<int> GetWeekendsLifeExpectancy(WeekendsLeftRequest weekendsLeftRequest)
        {
            var responseString = await client.GetStringAsync("https://d6wn6bmjj722w.population.io/1.0/life-expectancy/remaining/female/New%20Zealand/2020-09-30/45y9m/");

            return 42;
        }

    }
}
