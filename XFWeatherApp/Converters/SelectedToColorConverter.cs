using System;
using Xamarin.Forms;

namespace XFWeatherApp.Converters
{
    public class SelectedToColorConverter : IValueConverter
    {

        #region IValueConverter implementation
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool val && parameter is string param)
            {
                if (val)
                {
                    return Color.FromHex("#161763");
                }
                else
                {
                    switch (param)
                    {
                        case "Day":
                            return Color.White;
                        case "WeekDay":
                        default:
                            return Color.FromHex("#B1B3CD");
                    }
                }
            }
            return Color.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
