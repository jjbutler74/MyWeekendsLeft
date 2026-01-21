using System;
using System.Net.Http;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MWL.Services.Implementation;
using MWL.Services.Interface;
using Polly;
using Polly.Extensions.Http;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MWL.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Validate critical configuration at startup
            ValidateConfiguration();

            services.AddControllers();
            services.AddScoped<IWeekendsLeftService, WeekendsLeftService>();
            services.AddScoped<ICountriesService, CountriesService>();
            services.AddScoped<ILifeExpectancyService, LifeExpectancyService>();
            services.AddMemoryCache();

            // Add response compression (gzip and brotli)
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });

            // Add CORS policy
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            // Configure HttpClient with timeout and retry policies
            services.AddHttpClient<ILifeExpectancyService, LifeExpectancyService>()
                .ConfigureHttpClient(client =>
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                })
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy());

            services.AddHealthChecks();

            services.AddApiVersioning(
                options =>
                {
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = new ApiVersion(1,0);
                    options.ReportApiVersions = true; // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
                })
                .AddApiExplorer(
                options =>
                {
                    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service (specified format code will format the version as "'v'major[.minor][-status]")
                    options.GroupNameFormat = "'v'VVV";
                });
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(
                options =>
                {
                    options.OperationFilter<SwaggerDefaultValues>(); // add a custom operation filter which sets default values
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable response compression
            app.UseResponseCompression();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Weekends Left API");
                c.RoutePrefix = string.Empty;  // Set Swagger UI at apps root
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            // Enable CORS
            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }

        private void ValidateConfiguration()
        {
            var lifeExpectancyApiUri = Configuration.GetValue<string>("MwlConfiguration:LifeExpectancyApiUri");

            if (string.IsNullOrWhiteSpace(lifeExpectancyApiUri))
            {
                throw new InvalidOperationException(
                    "Configuration error: 'MwlConfiguration:LifeExpectancyApiUri' is required but not configured in appsettings.json");
            }

            if (!Uri.TryCreate(lifeExpectancyApiUri, UriKind.Absolute, out var uri))
            {
                throw new InvalidOperationException(
                    $"Configuration error: 'MwlConfiguration:LifeExpectancyApiUri' is not a valid URI: {lifeExpectancyApiUri}");
            }
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            // Retry 3 times with exponential backoff (2^attempt seconds: 2s, 4s, 8s)
            return HttpPolicyExtensions
                .HandleTransientHttpError() // Handles 5xx and 408 responses
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound) // Also retry 404s
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            // Break circuit after 5 consecutive failures, stay open for 30 seconds
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }
    }
}
