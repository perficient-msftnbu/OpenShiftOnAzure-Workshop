namespace Perficient.OpenShift.WorkShop.DataSeeder.Providers
{
    public interface IDataSeederProvider
    {
        void InsertWeatherForecasts();
        bool HasWeatherForecastsData();
    }
}
