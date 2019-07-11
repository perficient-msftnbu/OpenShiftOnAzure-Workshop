using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Perficient.OpenShift.Workshop.Models;
using Perficient.OpenShift.WorkShop.DataSeeder.Providers;
using System;
using System.Threading.Tasks;

namespace Perficient.OpenShift.WorkShop.DataSeeder
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Initializing the Weather Forecasts data seeder...");

            var host = new HostBuilder()
                .ConfigureAppConfiguration(configHost =>
                {
                    configHost.AddJsonFile("appsettings.json");
                    configHost.AddUserSecrets<Program>(optional: true);
                    configHost.AddEnvironmentVariables();
                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.AddConsole();
                    configLogging.AddDebug();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging();
                    services.AddOptions();
                    var serviceProvider = services.BuildServiceProvider();

                    // Get the config settings for the Mongo DB provider
                    var mongoDbSettings = new MongoDbSettings();
                    hostContext.Configuration.Bind(nameof(MongoDbSettings), mongoDbSettings);

                    // Add services
                    var useMockData = hostContext.Configuration.GetValue<bool>("UseMockData");
                    if (useMockData)
                    {
                        services.AddSingleton<IDataSeederProvider, MockDataSeedProvider>();
                    }
                    else
                    {
                        // Override the existing MongoDbSettings with the mongodb RedHat 
                        // template environment variable values (we have to do this since
                        // we do not have control of the environment variable names)
                        mongoDbSettings.DatabaseName = hostContext.Configuration.GetValue<string>("DATABASE_NAME");
                        mongoDbSettings.Username = hostContext.Configuration.GetValue<string>("DATABASE_USER");
                        mongoDbSettings.Password = hostContext.Configuration.GetValue<string>("DATABASE_PASSWORD");
                        var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                        var logger = loggerFactory.CreateLogger<IDataSeederProvider>();
                        services.AddTransient<IDataSeederProvider>(provider => new MongoDbDataSeedProvider(logger, mongoDbSettings));
                    }

                    // Put this here so the IDataSeederProvider instance will already be there to inject
                    services.AddHostedService<DataSeedService>();
                })
                .UseConsoleLifetime()
                .Build();

            await host.RunAsync();
            host.Dispose();
        }
    }
}
