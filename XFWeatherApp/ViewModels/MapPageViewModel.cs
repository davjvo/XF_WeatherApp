using Acr.UserDialogs;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using XFWeatherApp.ApiManagers.Maps;
using XFWeatherApp.Models;
using XFWeatherApp.Utils;

namespace XFWeatherApp.ViewModels
{
    public class MapPageViewModel : BaseViewModel, INavigatedAware
    {
        public bool IsSearching { get; set; }
        public string SearchText { get; set; }
        public MapPosition Position { get; set; }
        public Autocomplete Selected { get; set; }
        public ObservableCollection<Autocomplete> Suggestions { get; set; }
        public DelegateCommand SelectAutocompleteCommand { get; set; }
        public DelegateCommand SelectCommmand { get; set; }
        public DelegateCommand SearchCommand { get; set; }
        readonly IMapsApiManager _mapsApiManager;
        readonly INavigationService _navigationService;
        public MapPageViewModel(IUserDialogs userDialogs, IMapsApiManager mapsApiManager, INavigationService navigationService) : base(userDialogs)
        {
            _mapsApiManager = mapsApiManager;
            _navigationService = navigationService;
            Suggestions = new ObservableCollection<Autocomplete>();
            SearchCommand = new DelegateCommand(async () => await Search());
            SelectAutocompleteCommand = new DelegateCommand(async () => await SetSelectedValues());
            SelectCommmand = new DelegateCommand(async () => await Submit());
        }
        async Task Search()
        {
            try
            {
                IsSearching = SearchText.Length > 0 && !string.IsNullOrEmpty(Position?.Location);
                var mapsSuggestion = await _mapsApiManager.GetAutoComplete(SearchText);
                Suggestions = new ObservableCollection<Autocomplete>(mapsSuggestion);
            }
            catch (Exception ex)
            {
                await HandleError(ex);
            }
        }
        async Task SetSelectedValues()
        {
            try
            {
                if (Selected != null)
                {
                    var detail = await _mapsApiManager.GetPlaceDetail(Selected.PlaceId);
                    Position = new MapPosition(detail.Latitude, detail.Longitude, Selected.Description);
                }
                IsSearching = false;
            }
            catch (Exception ex)
            {
                await HandleError(ex);
            }
        }
        async Task Submit()
        {
            await _navigationService.GoBackAsync(new NavigationParameters
            {
                { ParamKeys.Latitude, Position.Latitude },
                { ParamKeys.Longitude, Position.Longitude },
                { ParamKeys.Location, Position.Location }
            });
        }
        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }
        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if(parameters.ContainsKey(ParamKeys.Latitude) && parameters.ContainsKey(ParamKeys.Longitude) && parameters.ContainsKey(ParamKeys.Location))
            {
                var latitude = (double)parameters[ParamKeys.Latitude];
                var longitude = (double)parameters[ParamKeys.Longitude];
                var location = SearchText = (string)parameters[ParamKeys.Location];
                Position = new MapPosition(latitude, longitude, location);
            }
        }
    }
}
