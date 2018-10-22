using System;
using System.Linq;

namespace WindSeeker.Web.NationalWeatherService
{
    public class StationObservations
    {
        public StationObservations(Station station, Observation[] observations)
        {
            Station = station;
            Observations = observations;

            ObservationCount = observations.Length;
            StartDate = observations.Min(x => x.Timestamp);
            EndDate = observations.Max(x => x.Timestamp);

            // todo probably faster to just iterate once over observations
            var hasTemperature = observations.Where(x => x.Temperature.HasValue).Select(x => x.Temperature).Cast<double>();
            TemperatureObservationCount = hasTemperature.Count();
            MinTemperature = hasTemperature.Min();
            MaxTemperature = hasTemperature.Max();
            AverageTemperature = hasTemperature.Average();

            var hasWindSpeed = observations.Where(x => x.WindSpeed.HasValue).Select(x => x.WindSpeed).Cast<double>();
            WindSpeedObservationCount = hasWindSpeed.Count();
            MinWindSpeed = hasWindSpeed.Min();
            MaxWindSpeed = hasWindSpeed.Max();
            AverageWindSpeed = hasWindSpeed.Average();

            var hasWindGust = observations.Where(x => x.WindGust.HasValue).Select(x => x.WindGust).Cast<double>();
            WindGustObservationCount = hasWindGust.Count();
            MinWindGust = hasWindGust.Min();
            MaxWindGust = hasWindGust.Max();
            AverageWindGust = hasWindGust.Average();
        }

        public Station Station { get; }
        public Observation[] Observations { get; }

        public int ObservationCount { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }

        public int TemperatureObservationCount { get; }
        public double MinTemperature { get; }
        public double MaxTemperature { get; }
        public double AverageTemperature { get; }

        public int WindSpeedObservationCount { get; }
        public double MinWindSpeed { get; }
        public double MaxWindSpeed { get; }
        public double AverageWindSpeed { get; }

        public int WindGustObservationCount { get; }
        public double MinWindGust { get; }
        public double MaxWindGust { get; }
        public double AverageWindGust { get; }
    }
}