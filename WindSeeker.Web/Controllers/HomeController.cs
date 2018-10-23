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

        //[HttpGet]
        //public async Task<IActionResult> Index(FindStationsViewModel vm)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return await Task.FromResult(View());
        //        }

        //        return RedirectToAction(nameof(List), new {vm.Latitude, vm.Longitude});
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, ex.Message);
        //        throw;
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> List(double latitude, double longitude)
        {
            try
            {
                // todo prevent this endpoint from being called directly and skipping lat/lon validation
                var stations = await _nwsClient.GetStations(latitude, longitude);
                var vm = new ListViewModel()
                {
                    Latitude = latitude,
                    Longitude = longitude,
                    Stations = stations
                };
                return View("List", vm);
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