using Microsoft.Extensions.Logging;
using Perficient.OpenShift.WorkShop.DataSeeder.Providers;

namespace Perficient.OpenShift.WorkShop.DataSeeder
{
    public class DataSeeder
    {
        private readonly ILogger<DataSeeder> logger;
        private readonly IDataSeederProvider dataSeederProvider;

        public DataSeeder(ILogger<DataSeeder> logger, IDataSeederProvider dataSeederProvider)
        {
            this.logger = logger;
            this.dataSeederProvider = dataSeederProvider;
        }

        public void Run()
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
        }
    }
}
