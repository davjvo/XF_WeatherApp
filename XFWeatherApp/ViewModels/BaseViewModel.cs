using Acr.UserDialogs;
using System.ComponentModel;

namespace XFWeatherApp.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        protected readonly IUserDialogs _userDialogs;
        public BaseViewModel(IUserDialogs userDialogs)
        {
            _userDialogs = userDialogs;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
