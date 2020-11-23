using System;
using XFWeatherApp.Utils;

namespace XFWeatherApp.Models
{
    public class WeatherInfo
    {
        public string Location { get; set; }
        public string SkyStatus { get; set; }
        public string Description { get; set; }
        public double Temp { get; set; }
        public double FeelsLike { get; set; }
        public double Humidity { get; set; }
        public DateTime Date { get; set; }
        public Wind WindInfo { get; set; }
        public string Icon 
        {
            get 
            {
                if (SkyStatus == "Thunderstorm")
                    return IconFont.CloudShowersHeavy;

                var currentDate = DateTime.Now;

                if(currentDate.Hour > 6 && currentDate.Hour < 18)
                {
                    switch (SkyStatus)
                    {
                        case "Clear":
                            return IconFont.Sun;
                        case "Clouds":
                            return IconFont.CloudSun;
                        case "Rain":
                            return IconFont.CloudSunRain;
                        default:
                            return string.Empty;
                    }
                }
                else
                {
                    switch (SkyStatus)
                    {
                        case "Clear":
                            return IconFont.Moon;
                        case "Clouds":
                            return IconFont.CloudMoon;
                        case "Rain":
                            return IconFont.CloudMoonRain;
                        default:
                            return string.Empty;
                    }
                }
            }
        }
        public class Wind
        {
            public double Speed { get; set; }
            public double Degree { get; set; }
        }
    }
}
