using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using WindSeeker.Web.NationalWeatherService;

namespace WindSeeker.Tests
{
    [TestClass]
    public class NationalWeatherServiceClientTests
    {
        private static HttpClient _httpClient;
        private static INationalWeatherServiceClient _nwsClient;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            // todo centralize initialization of HttpClient so it's not repeated in web project startup
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri("http://api.weather.gov")
            };
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/geo+json"));
            // Have to fake it out so it sees the request as coming from a browser (Chrome latest)
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Safari/537.36");

            _nwsClient = new NationalWeatherServiceClient(new NullLogger<NationalWeatherServiceClient>(), _httpClient);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _httpClient?.Dispose();
        }


        [TestMethod]
        [DataRow(44.506806, -72.976629)]
        public async Task CanGetStations(double latitude, double longitude)
        {
            var stations = await _nwsClient.GetStations(latitude, longitude);
            Assert.IsNotNull(stations);
            Assert.IsTrue(stations.Any());
        }

        [TestMethod]
        [DataRow("KBTV")]
        public async Task CanGetStation(string stationIdentifier)
        {
            var station = await _nwsClient.GetStation(stationIdentifier);
            Assert.IsNotNull(station);
        }

        [TestMethod]
        [DataRow("xxx")]
        public async Task InvalidStationIdentifierReturnsNull(string stationIdentifier)
        {
            var station = await _nwsClient.GetStation(stationIdentifier);
            Assert.IsNull(station);
        }

        [TestMethod]
        [DataRow("KBTV", "2018-10-01", "2018-10-24")]
        public async Task CanGetObservations(string stationIdentifier, string startDate, string endDate)
        {
            var start = DateTime.Parse(startDate);
            var end = DateTime.Parse(endDate);
            var observations = await _nwsClient.GetObservations(stationIdentifier, start, end);
            Assert.IsNotNull(observations);
            Assert.IsTrue(observations.Any());
        }
    }
}