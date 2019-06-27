using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Perficient.OpenShift.Workshop.API.Configuration;
using Perficient.OpenShift.Workshop.API.Models;
using Perficient.OpenShift.Workshop.API.Providers.Interfaces;
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
            var database = this.GetDatabase();
            var forecastsCollection = database.GetCollection<WeatherForecast>("Forecasts");
            return forecastsCollection.Find(new BsonDocument()).ToList();
        }
    }
}
