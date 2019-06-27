using Microsoft.AspNetCore.Mvc;
using Perficient.OpenShift.Workshop.API.Models;
using Perficient.OpenShift.Workshop.API.Providers.Interfaces;
using System.Collections.Generic;

namespace Perficient.OpenShift.Workshop.API.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private readonly IWeatherForecastProvider weatherForecastProvider;

        public SampleDataController(IWeatherForecastProvider weatherForecastProvider)
        {
            this.weatherForecastProvider = weatherForecastProvider;
        }        

        [HttpGet("[action]")]
        public IEnumerable<WeatherForecast> WeatherForecasts(int startDateIndex)
        {
            return this.weatherForecastProvider.WeatherForecasts(startDateIndex);
        }
    }
}
