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

        public async Task<double> GetRemainingLifeExpectancy(WeekendsLeftRequest weekendsLeftRequest)
        {
            var response = await client.GetStringAsync("https://d6wn6bmjj722w.population.io/1.0/life-expectancy/remaining/female/New%20Zealand/2020-09-30/45y9m/");
            var responseDeserialize = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(response);
            return responseDeserialize.remaining_life_expectancy;
        }

        public WeekendsLeftResponse LifeExpectancyCalculations(int age, double remainingLifeExpectancy)
        {
            var weekendsLeftResponse = new WeekendsLeftResponse();

            var estimatedAgeOfDeath = (int)(age + remainingLifeExpectancy);
            var estimatedDaysLeft = (int)(remainingLifeExpectancy * 365);
            var estimatedDayOfDeath = DateTime.Now.AddDays(estimatedDaysLeft);
            var estimatedWeekendsLeft = estimatedDaysLeft / 7;
            
            weekendsLeftResponse.EstimatedAgeOfDeath = estimatedAgeOfDeath;
            weekendsLeftResponse.EstimatedDayOfDeath = estimatedDayOfDeath;
            weekendsLeftResponse.EstimatedWeekendsLeft = estimatedWeekendsLeft;
            weekendsLeftResponse.Message = $"You have an estimated {estimatedWeekendsLeft} weekends left in your life, get out there and enjoy it!";
            return weekendsLeftResponse;
        }
    }
}
