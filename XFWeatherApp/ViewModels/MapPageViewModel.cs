using Acr.UserDialogs;
using Prism.Commands;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using XFWeatherApp.ApiManagers.Maps;
using XFWeatherApp.Models;

namespace XFWeatherApp.ViewModels
{
    public class MapPageViewModel : BaseViewModel, INavigatedAware
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string SearchText { get; set; }
        public bool IsSearching { get; set; }
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
            SelectAutocompleteCommand = new DelegateCommand(SetSelected);
            SelectCommmand = new DelegateCommand(async () => await Submit());
        }
        async Task Search()
        {
            IsSearching = SearchText.Length > 0;
            var mapsSuggestion = await _mapsApiManager.GetAutoComplete(SearchText);
            Suggestions = new ObservableCollection<Autocomplete>(mapsSuggestion);
        }
        void SetSelected()
        {
            SearchText = Selected.Description;
            IsSearching = false;
        }
        async Task Submit()
        {
            var navParams = new NavigationParameters();
            if (Selected != null)
            {
                var detail = await _mapsApiManager.GetPlaceDetail(Selected.PlaceId);
                navParams.Add("Latitude", detail.Lat.ToString());
                navParams.Add("Longitude", detail.Lon.ToString());
                navParams.Add("Location", SearchText);
            }
            await _navigationService.GoBackAsync(navParams);
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if(parameters.ContainsKey("Latitude") && parameters.ContainsKey("Longitude"))
            {
                Latitude = (double)parameters["Latitude"];
                Longitude = (double)parameters["Longitude"];
            }
        }
    }
}
