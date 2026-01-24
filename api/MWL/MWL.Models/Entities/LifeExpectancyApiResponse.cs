using System.Text.Json.Serialization;

namespace MWL.Models.Entities
{
    /// <summary>
    /// Response model for the population.io life expectancy API.
    /// </summary>
    public class LifeExpectancyApiResponse
    {
        [JsonPropertyName("remaining_life_expectancy")]
        public double RemainingLifeExpectancy { get; set; }

        [JsonPropertyName("sex")]
        public string? Sex { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("date")]
        public string? Date { get; set; }

        [JsonPropertyName("age")]
        public string? Age { get; set; }
    }
}
