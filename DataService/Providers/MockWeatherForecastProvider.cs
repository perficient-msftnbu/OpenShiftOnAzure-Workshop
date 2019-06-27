using Perficient.OpenShift.Workshop.API.Models;
using Perficient.OpenShift.Workshop.API.Providers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Perficient.OpenShift.Workshop.API.Providers
{
    public class MockWeatherForecastProvider : IWeatherForecastProvider
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public IEnumerable<WeatherForecast> WeatherForecasts(int startDateIndex)
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                DateFormatted = DateTime.Now.AddDays(index + startDateIndex).ToString("d"),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
        }
    }
}
