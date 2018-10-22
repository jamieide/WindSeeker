using WindSeeker.Web.NationalWeatherService;

namespace WindSeeker.Web.Models
{
    public class ListViewModel
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Station[] Stations { get; set; }
    }
}