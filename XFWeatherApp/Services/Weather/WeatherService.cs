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
            Api = RestService.For<IWeatherService>(AppSetting.WeatherHost);
        }
        public async Task<HttpResponseMessage> GetByLatLon(string lat, string lon, string appid)
        {
            var request = Api.GetByLatLon(lat, lon, appid);
            return await ExecRequest(request);
        }
        public async Task<HttpResponseMessage> GetForecastByLatLon(string lat, string lon, string appid)
        {
            var request = Api.GetForecastByLatLon(lat, lon, appid);
            return await ExecRequest(request);
        }
    }
}