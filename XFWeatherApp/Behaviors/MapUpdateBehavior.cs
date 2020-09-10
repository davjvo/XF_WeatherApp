using Prism.Behaviors;
using System;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using XFWeatherApp.Models;

namespace XFWeatherApp.Behaviors
{
    public class MapUpdateBehavior : BehaviorBase<Map>
    {
        public static readonly BindableProperty PositionProperty = BindableProperty.Create("Position", typeof(MapPosition), typeof(MapUpdateBehavior), null, propertyChanged: OnPositionChange);
        public MapPosition Position
        {
            get { return (MapPosition)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }
        protected override void OnAttachedTo(Map bindable)
        {
            base.OnAttachedTo(bindable);
        }
        protected override void OnDetachingFrom(Map bindable)
        {
            base.OnDetachingFrom(bindable);
        }
        static void OnPositionChange(BindableObject bindable, object oldValue, object newValue)
        {
            var mapBehavior = bindable as MapUpdateBehavior;

            if (mapBehavior.Position != null && mapBehavior.AssociatedObject is Map map)
            {
                var newPosition = new Position(mapBehavior.Position.Latitude, mapBehavior.Position.Longitude);
                var cameraPosition = new CameraPosition(newPosition, 12);
                map.AnimateCamera(CameraUpdateFactory.NewCameraPosition(cameraPosition), duration: TimeSpan.FromSeconds(2));
            }

        }
    }
}
