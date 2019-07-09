using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Perficient.OpenShift.Workshop.API.Providers.Interfaces;
using Perficient.OpenShift.Workshop.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Perficient.OpenShift.Workshop.API.Providers
{
    public class MongoDbWeatherForecastProvider : IWeatherForecastProvider
    {
        private readonly MongoDbSettings settings;
        private readonly ILogger logger;
        private IMongoDatabase database;

        public MongoDbWeatherForecastProvider(MongoDbSettings settings,
            ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger), $"{nameof(logger)} cannot be null.");
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings), $"{nameof(settings)} cannot be null.");
        }

        /// <summary>
        /// Lazy-load for the MongoDB database driver.
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

        public IEnumerable<WeatherForecast> WeatherForecasts(int startDateIndex)
        {
            var weatherForecasts = new List<WeatherForecast>();
            var database = this.GetDatabase();
            var forecastsCollection = database.GetCollection<BsonDocument>(nameof(WeatherForecast));
            var documents = forecastsCollection.Find(new BsonDocument()).ToList();
            foreach(var document in documents)
            {
                weatherForecasts.Add(new WeatherForecast
                {
                    DateFormatted = document.GetValue(nameof(WeatherForecast.DateFormatted)).AsString,
                    TemperatureC = document.GetValue(nameof(WeatherForecast.TemperatureC)).AsInt32,
                    Summary = document.GetValue(nameof(WeatherForecast.Summary)).AsString
                });
            }
            return weatherForecasts;
        }
    }
}
