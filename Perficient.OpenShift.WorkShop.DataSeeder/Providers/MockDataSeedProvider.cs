using Microsoft.Extensions.Logging;

namespace Perficient.OpenShift.WorkShop.DataSeeder.Providers
{
    public class MockDataSeedProvider : IDataSeederProvider
    {
        private readonly ILogger<MockDataSeedProvider> logger;

        public MockDataSeedProvider(ILogger<MockDataSeedProvider> logger)
        {
            this.logger = logger;
        }

        public bool HasWeatherForecastsData()
        {
            return true;
        }

        public void InsertWeatherForecasts()
        {
            this.logger.LogInformation("MOCK:  Inserting the weather forecasts data...");
        }
    }
}
