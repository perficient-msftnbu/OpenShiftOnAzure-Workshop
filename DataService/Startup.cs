using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Perficient.OpenShift.Workshop.API.Configuration;
using Perficient.OpenShift.Workshop.API.Providers;
using Perficient.OpenShift.Workshop.API.Providers.Interfaces;

namespace Perficient.OpenShift.Workshop.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Get the config settings (environment variables) for the Mongo DB provider
            services.Configure<MongoDbSettings>(this.Configuration.GetSection(nameof(MongoDbSettings)));

            // Wire up the Weather Forecast provider implementation
            services.AddSingleton<IWeatherForecastProvider, MockWeatherForecastProvider>();
            //services.AddTransient<IWeatherForecastProvider, MongoDbWeatherForecastProvider>();
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
