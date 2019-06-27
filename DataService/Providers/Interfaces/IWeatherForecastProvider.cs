using Perficient.OpenShift.Workshop.API.Models;
using System.Collections.Generic;

namespace Perficient.OpenShift.Workshop.API.Providers.Interfaces
{
    public interface IWeatherForecastProvider
    {
        IEnumerable<WeatherForecast> WeatherForecasts(int startDateIndex);
    }
}
