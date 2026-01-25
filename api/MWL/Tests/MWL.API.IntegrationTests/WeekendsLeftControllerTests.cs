using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using MWL.Models;
using Xunit;

namespace MWL.API.IntegrationTests
{
    public class WeekendsLeftControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public WeekendsLeftControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        [Trait("Category", "API-Integration")]
        public async Task GetWeekends_WithValidRequest_Returns200Ok()
        {
            // Arrange
            var age = 45;
            var gender = "male";
            var country = "USA";

            // Act
            var response = await _client.GetAsync($"/api/getweekends/?age={age}&gender={gender}&country={country}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<WeekendsLeftResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(result);
            Assert.True(result.EstimatedWeekendsLeft > 0);
            Assert.True(result.EstimatedAgeOfDeath > age);
            Assert.NotNull(result.Message);
        }

        [Theory]
        [Trait("Category", "API-Integration")]
        [InlineData("invalid")]
        [InlineData("unknown")]
        [InlineData("")]
        [InlineData("xyz")]
        public async Task GetWeekends_WithInvalidGender_Returns400BadRequest(string gender)
        {
            // Arrange
            var age = 45;
            var country = "USA";

            // Act
            var response = await _client.GetAsync($"/api/getweekends/?age={age}&gender={gender}&country={country}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(problemDetails);
            Assert.Equal("Invalid Gender", problemDetails.Title);
            Assert.Contains("not valid", problemDetails.Detail);
        }

        [Theory]
        [Trait("Category", "API-Integration")]
        [InlineData(-5)]
        [InlineData(0)]
        [InlineData(121)]
        public async Task GetWeekends_WithInvalidAge_Returns400BadRequest(int age)
        {
            // Arrange
            var gender = "male";
            var country = "USA";

            // Act
            var response = await _client.GetAsync($"/api/getweekends/?age={age}&gender={gender}&country={country}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(problemDetails);
            Assert.Equal("Validation Failed", problemDetails.Title);
            Assert.Contains("Age", problemDetails.Detail);
        }

        [Theory]
        [Trait("Category", "API-Integration")]
        [InlineData("XX")]
        [InlineData("INVALID")]
        [InlineData("")]
        public async Task GetWeekends_WithInvalidCountryCode_Returns400BadRequest(string country)
        {
            // Arrange
            var age = 45;
            var gender = "male";

            // Act
            var response = await _client.GetAsync($"/api/getweekends/?age={age}&gender={gender}&country={country}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(problemDetails);
            Assert.Equal("Validation Failed", problemDetails.Title);
        }

        [Theory]
        [Trait("Category", "API-Integration")]
        [InlineData("Male")]
        [InlineData("male")]
        [InlineData("MALE")]
        [InlineData("Female")]
        [InlineData("female")]
        [InlineData("FEMALE")]
        public async Task GetWeekends_WithDifferentGenderCasing_Returns200Ok(string gender)
        {
            // Arrange
            var age = 45;
            var country = "USA";

            // Act
            var response = await _client.GetAsync($"/api/getweekends/?age={age}&gender={gender}&country={country}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        [Trait("Category", "API-Integration")]
        public async Task GetVersion_Returns200Ok()
        {
            // Act
            var response = await _client.GetAsync("/api/version/");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.NotNull(content);
            Assert.NotEmpty(content);
        }

        [Fact]
        [Trait("Category", "API-Integration")]
        public async Task HealthCheck_Returns200Ok()
        {
            // Act
            var response = await _client.GetAsync("/health");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal("Healthy", content);
        }

        [Fact]
        [Trait("Category", "API-Integration")]
        public async Task GetWeekends_ResponseHeaders_IncludeApiVersion()
        {
            // Arrange
            var age = 45;
            var gender = "male";
            var country = "USA";

            // Act
            var response = await _client.GetAsync($"/api/getweekends/?age={age}&gender={gender}&country={country}");

            // Assert
            Assert.True(response.Headers.Contains("api-supported-versions") ||
                       response.Headers.Contains("Api-Supported-Versions"));
        }

        [Theory]
        [Trait("Category", "API-Integration")]
        [InlineData("USA")]
        [InlineData("NZL")]
        [InlineData("GBR")]
        [InlineData("AUS")]
        public async Task GetWeekends_WithDifferentCountries_Returns200Ok(string country)
        {
            // Arrange
            var age = 45;
            var gender = "male";

            // Act
            var response = await _client.GetAsync($"/api/getweekends/?age={age}&gender={gender}&country={country}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<WeekendsLeftResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(result);
            Assert.True(result.EstimatedWeekendsLeft > 0);
        }

        [Fact]
        [Trait("Category", "API-Integration")]
        public async Task GetWeekends_ResponseContainsExpectedFields()
        {
            // Arrange
            var age = 45;
            var gender = "male";
            var country = "USA";

            // Act
            var response = await _client.GetAsync($"/api/getweekends/?age={age}&gender={gender}&country={country}");

            // Assert
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<WeekendsLeftResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(result);
            Assert.True(result.EstimatedAgeOfDeath > 0);
            Assert.True(result.EstimatedWeekendsLeft > 0);
            Assert.NotEqual(default(DateTime), result.EstimatedDayOfDeath);
            Assert.NotNull(result.Message);
            Assert.Contains("weekends", result.Message.ToLower());
        }

        [Fact]
        [Trait("Category", "API-Integration")]
        public async Task Response_ContainsSecurityHeaders()
        {
            // Act
            var response = await _client.GetAsync("/api/version/");

            // Assert - verify security headers are present
            Assert.True(response.Headers.Contains("X-Frame-Options"), "X-Frame-Options header should be present");
            Assert.True(response.Headers.Contains("X-Content-Type-Options"), "X-Content-Type-Options header should be present");
            Assert.True(response.Headers.Contains("X-XSS-Protection"), "X-XSS-Protection header should be present");
            Assert.True(response.Headers.Contains("Referrer-Policy"), "Referrer-Policy header should be present");

            // Verify header values
            Assert.Equal("DENY", response.Headers.GetValues("X-Frame-Options").First());
            Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").First());
        }

        [Fact]
        [Trait("Category", "API-Integration")]
        public async Task Response_ContainsContentSecurityPolicy()
        {
            // Act
            var response = await _client.GetAsync("/api/version/");

            // Assert - CSP is typically in Content headers, not response headers
            var hasCSP = response.Headers.Contains("Content-Security-Policy") ||
                        response.Content.Headers.Contains("Content-Security-Policy");

            Assert.True(hasCSP, "Content-Security-Policy header should be present");
        }
    }
}
