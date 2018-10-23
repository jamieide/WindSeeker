using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
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

        [HttpGet]
        public async Task<IActionResult> List(FindStationsViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(nameof(Index), vm);
                }

                var stations = await _nwsClient.GetStations(vm.Latitude, vm.Longitude);
                var vmList = new ListViewModel()
                {
                    Latitude = vm.Latitude,
                    Longitude = vm.Longitude,
                    Stations = stations
                };
                return View("List", vmList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            // return observations for last 30 days
            var end = DateTime.Now.Date;
            var start = end.AddDays(-30);

            try
            {
                var stationObservations = await _nwsClient.GetStationObservations(id, start, end);
                return View(stationObservations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}