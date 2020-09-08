using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(XFWeatherApp.CustomRenders.CustomEntry), typeof(XFWeatherApp.iOS.CustomRenders.CustomEntry))]
namespace XFWeatherApp.iOS.CustomRenders
{
    public class CustomEntry : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if(Control != null)
            {
                Control.BackgroundColor = UIColor.Clear;
                Control.BorderStyle = UITextBorderStyle.None;
            }
        }
    }
}