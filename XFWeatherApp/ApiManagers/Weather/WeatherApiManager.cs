using MonkeyCache.FileStore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XFWeatherApp.DTO;
using XFWeatherApp.Models;
using XFWeatherApp.Services.Weather;
using XFWeatherApp.Utils;

namespace XFWeatherApp.ApiManagers.Weather
{
    public class WeatherApiManager : IWeatherApiManager
    {
        readonly IWeatherService _weatherService;
        const string LastWeatherReportKey = "WeatherReport";
        const string LastForecastKey = "Forecast";
        public WeatherApiManager(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }
        public async Task<IEnumerable<WeatherInfo>> GetForecast(string lat, string lon)
        {
            var forecastResponse = await _weatherService.GetForecastByLatLon(lat, lon, AppSetting.WeatherHost, AppSetting.WeatherKey);
            IEnumerable<WeatherInfo> forecastInfo = new List<WeatherInfo>();
            if (forecastResponse.IsSuccessStatusCode)
            {
                var json = await forecastResponse.Content.ReadAsStringAsync();
                var forecast = JsonConvert.DeserializeObject<ForecastResponse>(json);
                forecastInfo = forecast.List.Select(f => new WeatherInfo
                {
                    Date = Helper.UnixTimeStampToDateTime(f.Dt),
                    FeelsLike = f.Main.FeelsLike,
                    Humidity = f.Main.Humidity,
                    Temp = f.Main.Temp,
                    SkyStatus = f.Weather[0]?.Main,
                    WindInfo = new WeatherInfo.Wind
                    {
                        Speed = f.Wind.Speed,
                        Degree = f.Wind.Deg,
                    }
                });

                Barrel.Current.Add(LastForecastKey, forecast, TimeSpan.FromDays(1));
            }
            else
            {
                var cached = Barrel.Current.Get<ForecastResponse>(LastForecastKey);
                if(cached != null)
                {
                    forecastInfo = cached.List.Select(f => new WeatherInfo
                    {
                        Date = Helper.UnixTimeStampToDateTime(f.Dt),
                        FeelsLike = f.Main.FeelsLike,
                        Humidity = f.Main.Humidity,
                        Temp = f.Main.Temp,
                        SkyStatus = f.Weather[0]?.Main,
                        WindInfo = new WeatherInfo.Wind
                        {
                            Speed = f.Wind.Speed,
                            Degree = f.Wind.Deg,
                        }
                    });
                }
            }

            return forecastInfo;
        }

        public async Task<WeatherInfo> GetWeatherInfo(string lat, string lon)
        {
            var weatherResponse = await _weatherService.GetByLatLon(lat, lon, AppSetting.WeatherHost, AppSetting.WeatherKey);
            var weatherInfo = new WeatherInfo();
            if (weatherResponse.IsSuccessStatusCode)
            {
                var json = await weatherResponse.Content.ReadAsStringAsync();
                var weather = JsonConvert.DeserializeObject<WeatherResponse>(json);
                weatherInfo.Location = weather.Name;
                weatherInfo.Date = Helper.UnixTimeStampToDateTime(weather.Dt);
                weatherInfo.FeelsLike = weather.Main.FeelsLike;
                weatherInfo.Humidity = weather.Main.Humidity;
                weatherInfo.Temp = weather.Main.Temp;
                weatherInfo.SkyStatus = weather.Weather[0]?.Main;
                weatherInfo.WindInfo = new WeatherInfo.Wind
                {
                    Speed = weather.Wind.Speed,
                    Degree = weather.Wind.Deg,
                };

                Barrel.Current.Add(LastWeatherReportKey, weather, TimeSpan.FromDays(1));
            }
            else
            {
                var cached = Barrel.Current.Get<WeatherResponse>(LastWeatherReportKey);
                if(cached != null)
                {
                    weatherInfo.Location = cached.Name;
                    weatherInfo.Date = Helper.UnixTimeStampToDateTime(cached.Dt);
                    weatherInfo.FeelsLike = cached.Main.FeelsLike;
                    weatherInfo.Humidity = cached.Main.Humidity;
                    weatherInfo.Temp = cached.Main.Temp;
                    weatherInfo.SkyStatus = cached.Weather[0]?.Main;
                    weatherInfo.WindInfo = new WeatherInfo.Wind
                    {
                        Speed = cached.Wind.Speed,
                        Degree = cached.Wind.Deg,
                    };
                }
            }

            return weatherInfo;
        }
    }
}
