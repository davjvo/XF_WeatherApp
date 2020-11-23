using Acr.UserDialogs;
using Plugin.Connectivity;
using Prism;
using Prism.Ioc;
using Prism.Unity;
using XFWeatherApp.ApiManagers.Weather;
using XFWeatherApp.Services.Weather;
using XFWeatherApp.ViewModels;
using XFWeatherApp.Views;

namespace XFWeatherApp
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer initializer = null) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync("/HomePage");
        }
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //Helpers
            containerRegistry.RegisterInstance(UserDialogs.Instance);
            containerRegistry.RegisterInstance(CrossConnectivity.Current);

            //Services
            containerRegistry.Register<IWeatherService, WeatherService>();

            //ApiManagers
            containerRegistry.Register<IWeatherApiManager, WeatherApiManager>();

            //Navigation
            containerRegistry.RegisterForNavigation<HomePage, HomePageViewModel>();
        }
    }
}
