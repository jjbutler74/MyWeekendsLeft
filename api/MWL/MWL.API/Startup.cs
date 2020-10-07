using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MWL.Services.Implementation;
using MWL.Services.Interface;
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
            services.AddControllers();
            services.AddScoped<IWeekendsLeftService, WeekendsLeftService>();
            services.AddScoped<ICountriesService, CountriesService>();
            services.AddScoped<ILifeExpectancyService, LifeExpectancyService>();
            services.AddMemoryCache();

            services.AddApiVersioning(
                options =>
                {
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = new ApiVersion(1,0);
                    options.ReportApiVersions = true; // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
                });
            services.AddVersionedApiExplorer(
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

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Weekends Left API");
                c.RoutePrefix = string.Empty;  // Set Swagger UI at apps root
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
