using Refit;
using System.Net.Http;
using System.Threading.Tasks;

namespace XFWeatherApp.Services.Weather
{
    public interface IWeatherService
    {
        [Get("/weather?units=metric")]
        Task<HttpResponseMessage> GetByLatLon(string lat, string lon, 
            [Header("x-rapidapi-host")] string rapidHost, 
            [Header("x-rapidapi-key")] string rapidKey);
        [Get("/forecast?units=metric")]
        Task<HttpResponseMessage> GetForecastByLatLon(string lat, string lon,
            [Header("x-rapidapi-host")] string rapidHost,
            [Header("x-rapidapi-key")] string rapidKey);
    }
}
