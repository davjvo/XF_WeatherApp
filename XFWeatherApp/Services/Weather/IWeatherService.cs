using Refit;
using System.Net.Http;
using System.Threading.Tasks;

namespace XFWeatherApp.Services.Weather
{
    public interface IWeatherService
    {
        [Get("/data/2.5/weather?units=metric")]
        Task<HttpResponseMessage> GetByLatLon(string lat, string lon, string appid);
        [Get("/data/2.5/forecast?units=metric")]
        Task<HttpResponseMessage> GetForecastByLatLon(string lat, string lon, string appid);
    }
}
