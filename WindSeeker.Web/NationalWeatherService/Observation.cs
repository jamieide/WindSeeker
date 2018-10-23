using System;

namespace WindSeeker.Web.NationalWeatherService
{
    /// <summary>
    /// A weather conditions observation.
    /// </summary>
    public class Observation
    {
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Temperature in degrees Celcius
        /// </summary>
        public double? Temperature { get; set; }

        /// <summary>
        /// Wind direction in degrees
        /// </summary>
        public int? WindDirection { get; set; }

        /// <summary>
        /// Wind speed in meters/second
        /// </summary>
        public double? WindSpeed { get; set; }

        /// <summary>
        /// Wind gust in meters/second
        /// </summary>
        public double? WindGust { get; set; }
    }
}