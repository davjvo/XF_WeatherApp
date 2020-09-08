using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(XFWeatherApp.CustomRenders.CustomEntry), typeof(XFWeatherApp.Droid.CustomRenders.CustomEntry))]
namespace XFWeatherApp.Droid.CustomRenders
{
    public class CustomEntry : EntryRenderer
    {
        public CustomEntry(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if(Control != null)
            {
                Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
            }
        }
    }
}