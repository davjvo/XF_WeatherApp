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
            get {
                switch (SkyStatus)
                {
                    case "Clouds":
                        return IconFont.Cloud;
                    case "Rain":
                    case "Thunderstorm":
                        return IconFont.CloudShowersHeavy;
                    case "Clear":
                        return IconFont.Sun;
                    default:
                        return string.Empty;
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
