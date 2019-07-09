using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Perficient.OpenShift.Workshop.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Perficient.OpenShift.WorkShop.DataSeeder.Providers
{
    public class MongoDbDataSeedProvider : IDataSeederProvider
    {
        private readonly ILogger logger;
        private readonly MongoDbSettings settings;
        private IMongoDatabase database;
        private readonly string[] summaries;
        private bool connectionError;

        public MongoDbDataSeedProvider(ILogger logger,
            MongoDbSettings settings)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger), $"{nameof(logger)} cannot be null.");
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings), $"{nameof(settings)} cannot be null.");
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
                var connectionString = this.settings.GetConnectionString();
                this.logger.LogInformation($"Connection String:  {connectionString}");
                var client = new MongoClient(connectionString);
                this.database = client.GetDatabase(this.settings.DatabaseName);
            }
            return this.database;
        }

        public bool HasWeatherForecastsData()
        {
            var hasWeatherForecastsData = false;
            try
            {
                var database = this.GetDatabase();
                var forecastsCollection = database.GetCollection<BsonDocument>(nameof(WeatherForecast));
                var documents = forecastsCollection.Find(new BsonDocument()).ToList();
                hasWeatherForecastsData = documents.Any();
            }
            catch(Exception e)
            {
                this.connectionError = true;
                this.logger.LogError(e, "Error getting weather forecasts data.");
            }
            return hasWeatherForecastsData;
        }

        public void InsertWeatherForecasts()
        {
            if (this.connectionError)
            {
                return;
            }
            try
            {
                var database = this.GetDatabase();
                var forecastsCollection = database.GetCollection<WeatherForecast>(nameof(WeatherForecast));
                var randomWeatherForecasts = this.GetRandomWeatherForecasts();
                foreach (var forecast in randomWeatherForecasts)
                {
                    forecastsCollection.InsertOne(forecast);
                }
            }
            catch(Exception e)
            {
                this.logger.LogError(e, "Error inserting weather forecasts data.");
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
