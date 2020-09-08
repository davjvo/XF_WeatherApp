using System.Collections.Generic;
using System.Threading.Tasks;
using XFWeatherApp.Models;

namespace XFWeatherApp.ApiManagers.Weather
{
    public interface IWeatherApiManager
    {
        Task<WeatherInfo> GetWeatherInfo(string lat, string lon);
        Task<IEnumerable<WeatherInfo>> GetForecast(string lat, string lon);
    }
}
