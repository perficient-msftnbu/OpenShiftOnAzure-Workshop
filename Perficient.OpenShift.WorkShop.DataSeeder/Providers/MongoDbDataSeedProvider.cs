using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Perficient.OpenShift.Workshop.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Perficient.OpenShift.WorkShop.DataSeeder.Providers
{
    public class MongoDbDataSeedProvider : IDataSeederProvider
    {
        private readonly ILogger<MongoDbDataSeedProvider> logger;
        private readonly IOptions<MongoDbSettings> settings;
        private IMongoDatabase database;
        private readonly string[] summaries;

        public MongoDbDataSeedProvider(ILogger<MongoDbDataSeedProvider> logger,
            IOptions<MongoDbSettings> settings)
        {
            this.logger = logger;
            this.settings = settings;
            this.summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };
        }

        /// <summary>
        /// Lazy-load for the MongoDB database.
        /// </summary>
        private IMongoDatabase GetDatabase()
        {
            if (this.database == null)
            {
                var client = new MongoClient(this.settings.Value.ConnectionString);
                this.database = client.GetDatabase(this.settings.Value.Database);
            }
            return this.database;
        }

        public bool HasWeatherForecastsData()
        {
            var forecastsCollection = this.database.GetCollection<WeatherForecast>(nameof(WeatherForecast));
            return forecastsCollection.AsQueryable().Any();
        }

        public void InsertWeatherForecasts()
        {
            var forecastsCollection = this.database.GetCollection<WeatherForecast>(nameof(WeatherForecast));
            var randomWeatherForecasts = this.GetRandomWeatherForecasts();
            foreach (var forecast in randomWeatherForecasts)
            {
                forecastsCollection.InsertOne(forecast);
            }
        }

        private IEnumerable<WeatherForecast> GetRandomWeatherForecasts()
        {
            var rng = new Random();
            var startDateIndex = 1;
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                DateFormatted = DateTime.Now.AddDays(index + startDateIndex).ToString("d"),
                TemperatureC = rng.Next(-20, 55),
                Summary = this.summaries[rng.Next(this.summaries.Length)]
            });
        }
    }
}
