using Acr.UserDialogs;
using Plugin.Connectivity.Abstractions;
using Refit;
using System.Net.Http;
using System.Threading.Tasks;
using XFWeatherApp.Utils;

namespace XFWeatherApp.Services.Map
{
    public class MapsServices : BaseService, IMapsServices
    {
        readonly IMapsServices Api;
        public MapsServices(IUserDialogs userDialogs, IConnectivity connectivity) : base(userDialogs, connectivity, AppSetting.MapsHost)
        {
            Api = RestService.For<IMapsServices>(AppSetting.MapsHost);
        }
        public async Task<HttpResponseMessage> GetAutocomplete(string input, string key)
        {
            var request = Api.GetAutocomplete(input, key);
            return await ExecRequest(request);
        }
        public async Task<HttpResponseMessage> GetPlaceDetail(string placeid, string key)
        {
            var request = Api.GetPlaceDetail(placeid, key);
            return await ExecRequest(request);
        }
    }
}
