using Acr.UserDialogs;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Essentials;
using XFWeatherApp.ApiManagers.Weather;
using XFWeatherApp.Models;
using XFWeatherApp.Utils;

namespace XFWeatherApp.ViewModels
{
    public class HomePageViewModel : BaseViewModel, INavigatedAware
    {
        public MapPosition Position { get; set; }
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
        }
        public async Task SetCurrentLocationAsync(bool permissionAsked = false)
        {
            try
            {
                var permissionStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (permissionStatus == PermissionStatus.Granted)
                {
                    var location = await Geolocation.GetLocationAsync();
                    Position = new MapPosition(location.Latitude, location.Longitude, "");
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
            catch (Exception ex)
            {
                await HandleError(ex);
                _userDialogs.Toast("Unable to retrieve location.");
            }
        }
        public async Task FetchCurrentWeather()
        {
            try
            {
                var weatherInfo = await _weatherApiManager.GetWeatherInfo(Position?.Latitude.ToString(), Position?.Longitude.ToString());
                WeatherReport = new WeatherInfo
                {
                    Date = weatherInfo.Date,
                    FeelsLike = weatherInfo.FeelsLike,
                    Description = weatherInfo.Description,
                    Humidity = weatherInfo.Humidity,
                    Location =  weatherInfo.Location,
                    SkyStatus = weatherInfo.SkyStatus,
                    Temp = weatherInfo.Temp,
                    WindInfo = weatherInfo.WindInfo
                };

                Position.Location = WeatherReport.Location;
            }
            catch (Exception ex)
            {
                await HandleError(ex);
            }
        }
        public async Task FetchForecast()
        {
            try
            {
                var tempForecast = await _weatherApiManager.GetForecast(Position?.Latitude.ToString(), Position?.Longitude.ToString());
                Forecasts = new ObservableCollection<WeatherInfo>(tempForecast);
            }
            catch (Exception ex)
            {
                await HandleError(ex);
            }
        }
        public async Task GoToMap()
        {
            await _navigationService.NavigateAsync("MapPage", new NavigationParameters
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
            if (parameters.ContainsKey(ParamKeys.Latitude) && parameters.ContainsKey(ParamKeys.Longitude) && parameters.ContainsKey(ParamKeys.Location) )
            {
                var latitude = (double)parameters[ParamKeys.Latitude];
                var longitude = (double)parameters[ParamKeys.Longitude];
                var location = (string)parameters[ParamKeys.Location];

                Position = new MapPosition(latitude, longitude, location);

                GetCurrentWeatherCommand.Execute();
                GetForecastCommand.Execute();
            }
        }
    }
}
