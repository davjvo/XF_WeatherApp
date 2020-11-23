using System;
using System.ComponentModel;

namespace XFWeatherApp.Models
{
    public class WeatherDate : INotifyPropertyChanged
    {
        public DateTime Date { get; set; }
        public bool Selected { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}