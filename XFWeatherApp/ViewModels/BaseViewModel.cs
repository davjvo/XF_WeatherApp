using Acr.UserDialogs;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace XFWeatherApp.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        protected readonly IUserDialogs _userDialogs;
        public BaseViewModel(IUserDialogs userDialogs)
        {
            _userDialogs = userDialogs;
        }
        protected async Task HandleError(Exception ex)
        {
            if (ex.InnerException != null)
            {
                await HandleError(ex.InnerException);
                return;
            }

            await _userDialogs.AlertAsync(new AlertConfig
            {
                Message = $"Error: {ex.Message} \n StackTrace: {ex.StackTrace}",
                OkText = "Ok",
                Title = "Something went wrong :("
            });
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(PropertyChangedEventArgs eventArgs)
        {
            PropertyChanged?.Invoke(this, eventArgs);
        }
    }
}
