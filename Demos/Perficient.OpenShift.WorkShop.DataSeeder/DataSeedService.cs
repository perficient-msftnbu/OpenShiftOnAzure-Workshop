using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Perficient.OpenShift.WorkShop.DataSeeder.Providers;

namespace Perficient.OpenShift.WorkShop.DataSeeder
{
    public class DataSeedService : IHostedService
    {
        private readonly ILogger<DataSeedService> logger;
        private readonly IDataSeederProvider dataSeederProvider;

        public DataSeedService(ILogger<DataSeedService> logger, IDataSeederProvider dataSeederProvider)
        {
            this.logger = logger;
            this.dataSeederProvider = dataSeederProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Running the Weather Forecasts data seeder...");
            if (!this.dataSeederProvider.HasWeatherForecastsData())
            {
                this.logger.LogInformation("Setting the Weather Forecasts data...");
                this.dataSeederProvider.InsertWeatherForecasts();
            }
            else
            {
                this.logger.LogInformation("No changes - the Weather Forecasts data has previously been seeded.");
            }
            this.logger.LogInformation("Finished running the Weather Forecasts data seeder!");
            return Task.FromResult(0);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}
