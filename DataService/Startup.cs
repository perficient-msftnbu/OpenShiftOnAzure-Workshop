using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Perficient.OpenShift.Workshop.API.Providers;
using Perficient.OpenShift.Workshop.API.Providers.Interfaces;
using Perficient.OpenShift.Workshop.Models;

namespace Perficient.OpenShift.Workshop.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        private IConfiguration Configuration { get; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddOptions();

            // Get the config settings for the Mongo DB provider
            var mongoDbSettings = new MongoDbSettings();
            this.Configuration.Bind(nameof(MongoDbSettings), mongoDbSettings);

            // Wire up the Weather Forecast provider implementation
            var useMongoDb = this.Configuration.GetValue<bool>("UseMongoDb");
            if (useMongoDb)
            {
                // Update the existing IOptions<MongoDbSettings> with the mongodb RedHat 
                // template environment variable values
                var serviceProvider = services.BuildServiceProvider();
                mongoDbSettings.DatabaseName = this.Configuration.GetValue<string>("DATABASE_NAME");
                mongoDbSettings.Username = this.Configuration.GetValue<string>("DATABASE_USER");
                mongoDbSettings.Password = this.Configuration.GetValue<string>("DATABASE_PASSWORD");
                var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<IWeatherForecastProvider>();
                services.AddTransient<IWeatherForecastProvider>(provider => new MongoDbWeatherForecastProvider(mongoDbSettings, logger));
            }
            else
            {
                services.AddSingleton<IWeatherForecastProvider, MockWeatherForecastProvider>();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //don't think we need this as in a container world ssl termination usually happens at the router
            //app.UseHttpsRedirection();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });           
        }
    }
}
