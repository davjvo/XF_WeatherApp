using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XFWeatherApp.DTO;
using XFWeatherApp.Models;
using XFWeatherApp.Services.Map;
using XFWeatherApp.Utils;

namespace XFWeatherApp.ApiManagers.Maps
{
    public class MapsApiManager : IMapsApiManager
    {
        readonly IMapsServices _mapsService;
        public MapsApiManager(IMapsServices mapsService)
        {
            _mapsService = mapsService;
        }
        public async Task<IEnumerable<Autocomplete>> GetAutoComplete(string input)
        {
            var autocompleteResponse = await _mapsService.GetAutocomplete(input, AppSetting.MapsKey);
            var suggestions = Enumerable.Empty<Autocomplete>();
            if (autocompleteResponse.IsSuccessStatusCode)
            {
                var json = await autocompleteResponse.Content.ReadAsStringAsync();
                var autocomplete = JsonConvert.DeserializeObject<AutocompleteResponse>(json);
                suggestions = autocomplete.Predictions.Select(a => new Autocomplete
                {
                    Description = a.Description,
                    PlaceId = a.PlaceId
                });
            }

            return suggestions;
        }
        public async Task<Place> GetPlaceDetail(string placeId)
        {
            var placeResponse = await _mapsService.GetPlaceDetail(placeId, AppSetting.MapsKey);
            var place = new Place();
            if (placeResponse.IsSuccessStatusCode)
            {
                var json = await placeResponse.Content.ReadAsStringAsync();
                var detail = JsonConvert.DeserializeObject<DetailResponse>(json);
                place.Description = detail.Result.FormattedAddress;
                place.Lat = detail.Result.Geometry.Location.Lat;
                place.Lon = detail.Result.Geometry.Location.Lng;
            }

            return place;
        }
    }
}