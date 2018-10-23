using System;
using System.Threading.Tasks;

namespace WindSeeker.Web.NationalWeatherService
{
    /// <summary>
    /// Interface for NWS API client.
    /// </summary>
    public interface INationalWeatherServiceClient
    {
        Task<Station[]> GetStations(double latitude, double longitude);
        Task<Station> GetStation(string stationIdentifier);
        Task<Observation[]> GetObservations(string stationIdentifier, DateTime start, DateTime end);
        Task<StationObservations> GetStationObservations(string stationIdentifier, DateTime start, DateTime end);
    }
}