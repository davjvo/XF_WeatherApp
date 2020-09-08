using Acr.UserDialogs;
using Plugin.Connectivity.Abstractions;
using Refit;
using System.Net.Http;
using System.Threading.Tasks;
using XFWeatherApp.Utils;

namespace XFWeatherApp.Services.Weather
{
    public class WeatherService : BaseService, IWeatherService
    {
        readonly IWeatherService Api;
        public WeatherService(IUserDialogs userDialogs, IConnectivity connectivity) : base(userDialogs, connectivity, AppSetting.WeatherHost)
        {
            Api = RestService.For<IWeatherService>("https://community-open-weather-map.p.rapidapi.com");
        }
        public async Task<HttpResponseMessage> GetByLatLon(string lat, string lon, [Header("x-rapidapi-host")] string rapidHost, [Header("x-rapidapi-host")] string rapidKey)
        {
            var request = Api.GetByLatLon(lat, lon, rapidHost, rapidKey);
            return await ExecRequest(request);
        }
        public async Task<HttpResponseMessage> GetForecastByLatLon(string lat, string lon, [Header("x-rapidapi-host")] string rapidHost, [Header("x-rapidapi-host")] string rapidKey)
        {
            var request = Api.GetForecastByLatLon(lat, lon, rapidHost, rapidKey);
            return await ExecRequest(request);
        }
    }
}