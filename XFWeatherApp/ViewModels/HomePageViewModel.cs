using Acr.UserDialogs;
using Prism.Commands;
using Prism.Navigation;
using Prism.Xaml;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Essentials;
using XFWeatherApp.ApiManagers.Weather;
using XFWeatherApp.Models;

namespace XFWeatherApp.ViewModels
{
    public class HomePageViewModel : BaseViewModel, INavigatedAware
    {
        public string Location { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public ObservableCollection<WeatherInfo> Forecasts { get; set; } 
        public WeatherInfo WeatherReport { get; set; }
        public DelegateCommand GetCurrentWeatherCommand { get; set; }
        public DelegateCommand GetForecastCommand { get; set; }
        public DelegateCommand GetLastLocationCommand { get; set; }
        public DelegateCommand GoToMapCommand { get; set; }
        readonly IWeatherApiManager _weatherApiManager;
        public INavigationService _navigationService { get; set; }
        public HomePageViewModel(IWeatherApiManager weatherApiManager, INavigationService navigationService, IUserDialogs userDialogs) : base(userDialogs)
        {
            _weatherApiManager = weatherApiManager;
            _navigationService = navigationService;
            GetLastLocationCommand = new DelegateCommand(async () => await SetCurrentLocationAsync());
            GetCurrentWeatherCommand = new DelegateCommand(async () => await FetchCurrentWeather());
            GetForecastCommand = new DelegateCommand(async () => await FetchForecast());
            GoToMapCommand = new DelegateCommand(async () => await GoToMap());
            GetLastLocationCommand.Execute();
            GetCurrentWeatherCommand.Execute();
            GetForecastCommand.Execute();
        }
        public async Task SetCurrentLocationAsync(bool permissionAsked = false)
        {
            try
            {
                var permissionStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (permissionStatus == PermissionStatus.Granted)
                {
                    var location = await Geolocation.GetLastKnownLocationAsync();
                    Latitude = location.Latitude.ToString();
                    Longitude = location.Longitude.ToString();
                    GetCurrentWeatherCommand.Execute();
                    GetForecastCommand.Execute();
                }
                else if(!permissionAsked && (permissionStatus == PermissionStatus.Denied || permissionStatus == PermissionStatus.Unknown))
                {
                    await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    await SetCurrentLocationAsync(true);
                }
                else
                {
                    _userDialogs.Alert("The application can not work without your location.", "Yikes!", "Ok");
                }
                
            }
            catch (FeatureNotEnabledException)
            {
                _userDialogs.Toast("Device location seems to be turned off!.");
            }
            catch (PermissionException)
            {
                _userDialogs.Toast("The application can not work without your location.");
            }
            catch (FeatureNotSupportedException)
            {
                _userDialogs.Toast("This device doesn't support GeoLocation.");
            }
            catch (Exception)
            {
                _userDialogs.Toast("Unable to retrieve location.");
            }
        }
        public async Task FetchCurrentWeather()
        {
            try
            {
                var weatherInfo = await _weatherApiManager.GetWeatherInfo(Latitude, Longitude);
                WeatherReport = new WeatherInfo
                {
                    Date = weatherInfo.Date,
                    FeelsLike = weatherInfo.FeelsLike,
                    Description = weatherInfo.Description,
                    Humidity = weatherInfo.Humidity,
                    Location = Location ?? weatherInfo.Location,
                    SkyStatus = weatherInfo.SkyStatus,
                    Temp = weatherInfo.Temp,
                    WindInfo = weatherInfo.WindInfo
                };
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task FetchForecast()
        {
            try
            {
                var tempForecast = await _weatherApiManager.GetForecast(Latitude, Longitude);
                Forecasts = new ObservableCollection<WeatherInfo>(tempForecast);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task GoToMap()
        {
            var navParams = new NavigationParameters
            {
                { "Latitude", Latitude },
                { "Longitude", Longitude }
            };
            await _navigationService.NavigateAsync("MapPage", navParams);
        }
        public void OnNavigatedFrom(INavigationParameters parameters)
        {

        }
        public void OnNavigatedTo(INavigationParameters parameters)
        {
            var latitude = parameters.ContainsKey("Latitude") ? parameters["Latitude"] as string : string.Empty;
            var longitude = parameters.ContainsKey("Longitude") ? parameters["Longitude"] as string : string.Empty;
            var location = parameters.ContainsKey("Location") ? parameters["Location"] as string : string.Empty;
            if (!string.IsNullOrEmpty(latitude) && !string.IsNullOrEmpty(longitude))
            {
                Latitude = latitude;
                Longitude = longitude;
                Location = location;
                GetCurrentWeatherCommand.Execute();
                GetForecastCommand.Execute();
            }
        }
    }
}
