using System.ComponentModel.DataAnnotations;

namespace WindSeeker.Web.Models
{
    public class FindStationsViewModel
    {
        [Range(-90, 90)]
        public double Latitude { get; set; }

        [Range(-180, 180)]
        public double Longitude { get; set; }
    }
}