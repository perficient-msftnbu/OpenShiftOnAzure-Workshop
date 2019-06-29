using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging();
                    services.AddHostedService<DataSeedService>();
                    services.AddOptions();

                    // Get the config settings (environment variables) for the Mongo DB provider
                    services.Configure<MongoDbSettings>(hostContext.Configuration.GetSection(nameof(MongoDbSettings)));

                    // Add services
                    var useMockData = hostContext.Configuration.GetValue<bool>("UseMockData");
                    if (useMockData)
                    {
                        services.AddTransient<IDataSeederProvider, MockDataSeedProvider>();
                    }
                    else
                    {
                        services.AddTransient<IDataSeederProvider, MongoDbDataSeedProvider>();
                    }
                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.AddConsole();
                    configLogging.AddDebug();
                })
                .UseConsoleLifetime()
                .Build();

            await host.RunAsync();
            host.Dispose();
        }
    }
}
