using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace WindSeeker.Web.NationalWeatherService
{
    public class NationalWeatherServiceClient : INationalWeatherServiceClient
    {
        private readonly ILogger<NationalWeatherServiceClient> _logger;
        private readonly HttpClient _httpClient;

        public NationalWeatherServiceClient(ILogger<NationalWeatherServiceClient> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<Station[]> GetStations(double latitude, double longitude)
        {
            var uri = $"points/{latitude},{longitude}/stations";

            try
            {
                var response = await _httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                dynamic data = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                var stations = new List<Station>();
                foreach (var rawStation in data.features)
                {
                    var station = CreateStation(rawStation);
                    stations.Add(station);
                }

                return stations.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<Station> GetStation(string stationIdentifier)
        {
            // stationIdentifier is case sensitive
            var uri = $"stations/{stationIdentifier.ToUpperInvariant()}";

            try
            {
                var response = await _httpClient.GetAsync(uri);
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                response.EnsureSuccessStatusCode();
                dynamic data = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                return CreateStation(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        private static Station CreateStation(dynamic data)
        {
            return new Station()
            {
                Identifier = data.properties.stationIdentifier,
                Name = data.properties.name,
                TimeZone = data.properties.timeZone,
                Latitude = data.geometry.coordinates[0],
                Longitude = data.geometry.coordinates[1]
            };
        }

        /// <summary>
        /// Get observations for a station over a date range order by timestamp. The supplied dates are in the local time zone and the date portion is stripped.
        /// </summary>
        public async Task<Observation[]> GetObservations(string stationIdentifier, DateTime start, DateTime end)
        {
            // strip time portion and convert to ISO-8601 string in API expected format
            Func<DateTime, string> toApiDate = dt => dt.Date.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            Func<dynamic, DateTime> parseDate = v =>
            {
                // todo unit test conversion from UTC to local time -- it appears to be correct but...
                var s = Convert.ToString(v);
                return DateTime.Parse(s);
            };
            Func<dynamic, int?> parseInt = v =>
            {
                var s = Convert.ToString(v);
                if (int.TryParse(s, out int result))
                {
                    return result;
                }

                return null;
            };
            Func<dynamic, double?> parseDouble = v =>
            {
                var s = Convert.ToString(v);
                if (double.TryParse(s, out double result))
                {
                    return result;
                }

                return null;
            };
            
            var uri = $"stations/{stationIdentifier.ToUpperInvariant()}/observations?start={toApiDate(start)}&end={toApiDate(end)}";

            try
            {
                var response = await _httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                dynamic data = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                var observations = new List<Observation>();
                foreach (var rawObservation in data.features)
                {
                    var observation = new Observation()
                    {
                        Timestamp = parseDate(rawObservation.properties.timestamp),
                        Temperature = parseDouble(rawObservation.properties.temperature.value),
                        WindDirection = parseInt(rawObservation.properties.windDirection.value),
                        WindGust = parseDouble(rawObservation.properties.windGust.value),
                        WindSpeed = parseDouble(rawObservation.properties.windSpeed.value)
                    };
                    observations.Add(observation);
                }

                return observations.OrderBy(x => x.Timestamp).ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<StationObservations> GetStationObservations(string stationIdentifier, DateTime start, DateTime end)
        {
            var station = await GetStation(stationIdentifier);
            var observations = await GetObservations(stationIdentifier, start, end);
            return new StationObservations(station, observations);
        }

    }
}