namespace WindSeeker.Web.NationalWeatherService
{
    /// <summary>
    /// A NWS weather station.
    /// </summary>
    public class Station
    {
        public string Identifier { get; set; }

        public string Name { get; set; }

        public string TimeZone { get; set; }

        // todo The .NET Framework had a GeoCoordinate class but I can't find it in .NET Core
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public override string ToString() => $"{Identifier} {Name}";
    }
}