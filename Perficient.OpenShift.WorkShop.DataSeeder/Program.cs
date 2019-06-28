using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Perficient.OpenShift.Workshop.Models;
using Perficient.OpenShift.WorkShop.DataSeeder.Providers;
using System;

namespace Perficient.OpenShift.WorkShop.DataSeeder
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Initializing the Weather Forecasts data seeder...");

            // Create the service collection
            var serviceCollection = new ServiceCollection();
            Program.ConfigureServices(serviceCollection);

            // Create the service provider
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Run the app
            serviceProvider.GetService<DataSeeder>().Run();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Set up configuration
            // Note:  Since the secrets.json file will not exist on the build server, 
            // it is marked optional so it can just be ignored.
            var configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .AddUserSecrets<Program>(optional: true)
               .Build();
            services.AddOptions();

            // Get the config settings (environment variables) for the Mongo DB provider
            services.Configure<MongoDbSettings>(configuration.GetSection(nameof(MongoDbSettings)));

            // Add logging
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });

            // Add services
            services.AddTransient<IDataSeederProvider, MockDataSeedProvider>();

            // Add app
            services.AddTransient<DataSeeder>();
        }
    }
}
