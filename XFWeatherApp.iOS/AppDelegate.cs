using Foundation;
using Prism;
using Prism.Ioc;
using UIKit;
using Xamarin;
using Xamarin.Forms;

namespace XFWeatherApp.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Forms.SetFlags("Brush_Experimental");
            Forms.Init();
            FormsGoogleMaps.Init("GMAPS_KEY_HERE");
            LoadApplication(new App(new iOSInitializer()));

            return base.FinishedLaunching(app, options);
        }
        public class iOSInitializer : IPlatformInitializer
        {
            public void RegisterTypes(IContainerRegistry containerRegistry)
            {
            }
        }
    }
}
