using System.Collections.Generic;
using System.Threading.Tasks;
using XFWeatherApp.Models;

namespace XFWeatherApp.ApiManagers.Maps
{
    public interface IMapsApiManager
    {
        Task<IEnumerable<Autocomplete>> GetAutoComplete(string input);
        Task<Place> GetPlaceDetail(string placeId);
    }
}