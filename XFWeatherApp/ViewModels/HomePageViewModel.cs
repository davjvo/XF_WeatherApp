using Acr.UserDialogs;
using Microcharts;
using Prism.Commands;
using Prism.Navigation;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using XFWeatherApp.ApiManagers.Weather;
using XFWeatherApp.Models;

namespace XFWeatherApp.ViewModels
{
    public class HomePageViewModel : BaseViewModel, INavigatedAware
    {
        public Place UserLocation { get; set; }
        WeatherDate selectedDate;
        public WeatherDate SelectedDate
        {
            get { return selectedDate; }
            set
            {
                if (selectedDate != null)
                {
                    selectedDate.Selected = false;

                }

                if (value != null)
                {
                    selectedDate = value;
                    selectedDate.Selected = true;
                }
            }
        }
        public WeatherInfo CurrentDayWeatherReport { get; set; }
        public ObservableCollection<WeatherDate> Dates { get; set; }
        public List<WeatherInfo> Forecast { get; set; }
        public LineChart Chart { get; set; }
        public DelegateCommand GetCurrentWeatherCommand { get; set; }
        public DelegateCommand GetForecastCommand { get; set; }
        public DelegateCommand GetLastLocationCommand { get; set; }
        public DelegateCommand GoToMapCommand { get; set; }
        public DelegateCommand SelectedDateChangedCommand { get; set; }
        readonly IWeatherApiManager _weatherApiManager;
        public INavigationService _navigationService { get; set; }
        public HomePageViewModel(IWeatherApiManager weatherApiManager, INavigationService navigationService, IUserDialogs userDialogs) : base(userDialogs)
        {
            _weatherApiManager = weatherApiManager;
            _navigationService = navigationService;
            GetCurrentWeatherCommand = new DelegateCommand(async () => await FetchCurrentWeather());
            GetForecastCommand = new DelegateCommand(async () => await FetchForecast());
            SelectedDateChangedCommand = new DelegateCommand(async() => await SelectedDateChanged());
            Dates = new ObservableCollection<WeatherDate>();

            var initialDate = DateTime.Now;
            SelectedDate = new WeatherDate
            {
                Date = initialDate
            };
            Dates.Add(SelectedDate);
            for (int i = 1; i < 6; i++)
            {
                Dates.Add(new WeatherDate
                {
                    Date = initialDate.AddDays(i),
                });
            }

            Chart = new LineChart
            {
                BackgroundColor = SKColors.Transparent,
                PointMode = PointMode.Circle,
                LineMode = LineMode.Spline,
                PointSize = 50,
                AnimationDuration = TimeSpan.FromSeconds(2.3),
                LabelOrientation = Orientation.Horizontal,
                ValueLabelOrientation = Orientation.Horizontal,
                LabelColor = SKColors.White,
                LabelTextSize = 40,
            };
        }
        public async Task FetchCurrentWeather()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();
                var weatherInfo = await _weatherApiManager.GetWeatherInfo(location?.Latitude.ToString(), location?.Longitude.ToString());
                CurrentDayWeatherReport = new WeatherInfo
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
                UserLocation = new Place
                {
                    Description = CurrentDayWeatherReport.Location,
                    Latitude = location.Latitude,
                    Longitude = location.Longitude
                };
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
                var tempForecast = await _weatherApiManager.GetForecast(UserLocation?.Latitude.ToString(), UserLocation?.Longitude.ToString());
                Forecast = new List<WeatherInfo>(tempForecast);
                Chart.Entries = await ProcessForecast();
            }
            catch (Exception ex)
            {
                await HandleError(ex);
            }
        }
        async Task SelectedDateChanged()
        {
            Chart.Entries = await ProcessForecast();
        }
        async Task<List<ChartEntry>> ProcessForecast()
        {
            var entries = new List<ChartEntry>();
            try
            {
                var currentTime = SelectedDate.Date;
                foreach (var forecast in Forecast.Where(t => t.Date.Day == currentTime.Day))
                {
                    var entry = new ChartEntry((float)forecast.FeelsLike)
                    {
                        Label = forecast.Date.ToString("htt"),
                        ValueLabel = forecast.FeelsLike.ToString(),
                        ValueLabelColor = SKColors.White,
                        TextColor = SKColors.White,
                        Color = forecast.Date > currentTime && forecast.Date < currentTime.AddHours(3) ?
                                SKColor.Parse("#FA9B14") : SKColors.White,
                    };
                    entries.Add(entry);
                }
            }
            catch (Exception ex)
            {
                await HandleError(ex);
            }
            return entries;
        }
        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }
        public async void OnNavigatedTo(INavigationParameters parameters)
        {
            var permissionStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            while(!PermissionGranted(permissionStatus))
            {
                await _userDialogs.AlertAsync(new AlertConfig
                {
                    Message = "Oh no!, seems like we can't access your location. To continue please give this app the corresponding access",
                    OkText = "Ok",
                    Title = "Permission not granted",
                });
                await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                permissionStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            }

            if(UserLocation == null)
            {
                var geolocation = await Geolocation.GetLastKnownLocationAsync();
                UserLocation = new Place
                {
                    Latitude = geolocation.Latitude,
                    Longitude = geolocation.Longitude
                };
            }

            if(PermissionGranted(permissionStatus))
            {
                GetCurrentWeatherCommand.Execute();
                GetForecastCommand.Execute();
            }
        }
        bool PermissionGranted(PermissionStatus status)
        {
            return !(status == PermissionStatus.Denied || status == PermissionStatus.Unknown || status == PermissionStatus.Disabled);
        }
    }
}