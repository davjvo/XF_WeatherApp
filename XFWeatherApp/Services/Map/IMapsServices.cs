using Refit;
using System.Net.Http;
using System.Threading.Tasks;

namespace XFWeatherApp.Services.Map
{
    public interface IMapsServices
    {
        [Get("/maps/api/place/autocomplete/json")]
        Task<HttpResponseMessage> GetAutocomplete(string input, string key);
        [Get("/maps/api/place/details/json")]
        Task<HttpResponseMessage> GetPlaceDetail(string placeid, string key);
    }
}
