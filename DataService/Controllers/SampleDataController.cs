using CardApp.Models;
using CardApp.Providers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CardApp.Controllers
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
