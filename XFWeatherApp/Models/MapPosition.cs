namespace XFWeatherApp.Models
{
    public class MapPosition
    {
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public string Location { get; set; }

        public MapPosition(double latitude, double longitude, string location)
        {
            Latitude = latitude;
            Longitude = longitude;
            Location = location;
        }
    }
}
