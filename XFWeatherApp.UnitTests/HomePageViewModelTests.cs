using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XFWeatherApp.ApiManagers.Weather;
using XFWeatherApp.Models;
using XFWeatherApp.Utils;
using XFWeatherApp.ViewModels;

namespace XFWeatherApp.UnitTests
{
    [TestClass]
    public class HomePageViewModelTests
    {
        Mock<INavigationService> _navigationService;
        Mock<IWeatherApiManager> _weatherApiManager;
        HomePageViewModel _HomePageViewModel;

        [TestInitialize]
        public void Init()
        {
            _navigationService = new Mock<INavigationService>();
            _weatherApiManager = new Mock<IWeatherApiManager>();

            _HomePageViewModel = new HomePageViewModel(_weatherApiManager.Object, _navigationService.Object, null);
        }
        [TestCleanup]
        public void Cleanup()
        {
            _navigationService = null;
            _weatherApiManager = null;
            _HomePageViewModel = null;
        }
        [TestMethod]
        public void Forecast_Should_Be_Empty()
        {
            Assert.IsNull(_HomePageViewModel.Forecasts);
        }
        [TestMethod]
        public void Current_Should_Be_Empty()
        {
            Assert.IsNull(_HomePageViewModel.WeatherReport);
        }
        [TestMethod]
        public async Task Current_Should_Not_Be_Empty()
        {
            //Arrange
            _weatherApiManager
                .Setup(e => e.GetWeatherInfo(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new WeatherInfo
                {
                    Date = DateTime.Now,
                    Description = "Mock test",
                    FeelsLike = 1,
                    Humidity = 1,
                    Location = "Mock Location",
                    SkyStatus = "Rain",
                    Temp = 1,
                    WindInfo = new WeatherInfo.Wind
                    {
                        Degree = 1,
                        Speed = 1
                    }
                })
                .Verifiable();

            //Act
            await _HomePageViewModel.FetchCurrentWeather();

            //Assert
            _weatherApiManager.Verify(e => e.GetWeatherInfo(It.IsAny<string>(), It.IsAny<string>()));
            Assert.IsNotNull(_HomePageViewModel.WeatherReport);
        }
        [TestMethod]
        public async Task Forecast_Should_Not_Be_Empty()
        {
            //Arrange
            _weatherApiManager
                .Setup(e => e.GetForecast(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<WeatherInfo>
                {
                    new WeatherInfo
                    {
                        Date = DateTime.Now,
                        Description = "Mock test",
                        FeelsLike = 1,
                        Humidity = 1,
                        Location = "Mock Location",
                        SkyStatus = "Rain",
                        Temp = 1,
                        WindInfo = new WeatherInfo.Wind
                        {
                            Degree = 1,
                            Speed = 1
                        }
                    },
                    new WeatherInfo
                    {
                        Date = DateTime.Now,
                        Description = "Mock test",
                        FeelsLike = 1,
                        Humidity = 1,
                        Location = "Mock Location",
                        SkyStatus = "Rain",
                        Temp = 1,
                        WindInfo = new WeatherInfo.Wind
                        {
                            Degree = 1,
                            Speed = 1
                        }
                    }
                })
                .Verifiable();

            //Act
            await _HomePageViewModel.FetchForecast();

            //Assert
            _weatherApiManager.Verify(e => e.GetForecast(It.IsAny<string>(), It.IsAny<string>()));
            Assert.IsTrue(_HomePageViewModel.Forecasts.Count > 0);
        }
        [TestMethod]
        public async Task Current_Icon_Should_Be_Clouds()
        {
            //Arrange
            _weatherApiManager
                .Setup(e => e.GetWeatherInfo(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new WeatherInfo
                {
                    Date = DateTime.Now,
                    Description = "Mock test",
                    FeelsLike = 1,
                    Humidity = 1,
                    Location = "Mock Location",
                    SkyStatus = "Rain",
                    Temp = 1,
                    WindInfo = new WeatherInfo.Wind
                    {
                        Degree = 1,
                        Speed = 1
                    }
                })
                .Verifiable();

            //Act
            await _HomePageViewModel.FetchCurrentWeather();

            //Assert
            _weatherApiManager.Verify(e => e.GetWeatherInfo(It.IsAny<string>(), It.IsAny<string>()));
            Assert.AreEqual(IconFont.CloudShowersHeavy, _HomePageViewModel.WeatherReport.Icon);
        }
    }
}
