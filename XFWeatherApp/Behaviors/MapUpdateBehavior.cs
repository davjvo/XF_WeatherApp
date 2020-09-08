using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace XFWeatherApp.Behaviors
{
    public class MapUpdateBehavior : Behavior<Map>
    {
        public static readonly BindableProperty LongitudeProperty = BindableProperty.Create("Longitude", typeof(object), typeof(MapUpdateBehavior), null, propertyChanged: OnLongitudeChanged);
        public object Longitude
        {
            get { return (object)GetValue(LongitudeProperty); }
            set { SetValue(LongitudeProperty, value); }
        }
        protected override void OnAttachedTo(Map bindable)
        {
            base.OnAttachedTo(bindable);
        }
        protected override void OnDetachingFrom(Map bindable)
        {
            base.OnDetachingFrom(bindable);
        }
        static void OnLatitudeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var map = bindable as Map;
            if (map == null) return;
            CameraUpdate(map, (double)newValue, map.CameraPosition.Target.Longitude);
        }
        static void OnLongitudeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var map = bindable as Map;
            if (map == null) return;
            CameraUpdate(map, map.CameraPosition.Target.Latitude, (double)newValue);
        }

        static void CameraUpdate(Map map, double latitude, double longitude)
        {
        }
    }
}
