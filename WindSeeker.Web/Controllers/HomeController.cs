using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WindSeeker.Web.Models;
using WindSeeker.Web.NationalWeatherService;

namespace WindSeeker.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INationalWeatherServiceClient _nwsClient;

        public HomeController(ILogger<HomeController> logger, INationalWeatherServiceClient nwsClient)
        {
            _logger = logger;
            _nwsClient = nwsClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return await Task.FromResult(View());
        }

        [HttpPost]
        public async Task<IActionResult> Index(FindStationsViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }
                return RedirectToAction(nameof(List), vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> List(FindStationsViewModel findStationsViewModel)
        {
            try
            {
                var stations = await _nwsClient.GetStations(findStationsViewModel.Latitude, findStationsViewModel.Longitude);
                var vm = new ListViewModel()
                {
                    Latitude = findStationsViewModel.Latitude,
                    Longitude = findStationsViewModel.Longitude,
                    Stations = stations
                };
                return await Task.FromResult(View(vm));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(string stationIdentifier)
        {
            // return observations for last 30 days
            var end = DateTime.Now.Date;
            var start = end.AddDays(-30);

            try
            {
                var stationObservations = await _nwsClient.GetStationObservations(stationIdentifier, start, end);
                var vm = new DetailsViewModel()
                {
                    StationObservations = stationObservations
                };
                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}