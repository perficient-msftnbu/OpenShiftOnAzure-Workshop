using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Perficient.OpenShift.Workshop.API.Providers.Interfaces;
using Perficient.OpenShift.Workshop.Models;
using System.Collections.Generic;
using System.Linq;

namespace Perficient.OpenShift.Workshop.API.Providers
{
    public class MongoDbWeatherForecastProvider : IWeatherForecastProvider
    {
        private readonly IOptions<MongoDbSettings> settings;
        private IMongoDatabase database;

        public MongoDbWeatherForecastProvider(IOptions<MongoDbSettings> settings)
        {
            this.settings = settings;
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
