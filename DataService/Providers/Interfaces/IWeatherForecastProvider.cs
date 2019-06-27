using CardApp.Models;
using System.Collections.Generic;

namespace CardApp.Providers.Interfaces
{
    public interface IWeatherForecastProvider
    {
        IEnumerable<WeatherForecast> WeatherForecasts(int startDateIndex);
    }
}
