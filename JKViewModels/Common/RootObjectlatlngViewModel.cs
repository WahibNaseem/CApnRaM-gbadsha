using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Common
{
    /*Ready to use code :  simple copy paste GetLatLong*/
    public class AddressComponentlatlngViewModel
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; set; }
    }

    public class NortheastlatlngViewModel
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class SouthwestlatlngViewModel
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class BoundslatlngViewModel
    {
        public NortheastlatlngViewModel northeast { get; set; }
        public SouthwestlatlngViewModel southwest { get; set; }
    }

    public class LocationlatlngViewModel
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Northeast2latlngViewModel
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Southwest2latlngViewModel
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class ViewportlatlngViewModel
    {
        public Northeast2latlngViewModel northeast { get; set; }
        public Southwest2latlngViewModel southwest { get; set; }
    }

    public class Geometry
    {
        public BoundslatlngViewModel bounds { get; set; }
        public LocationlatlngViewModel location { get; set; }
        public string location_type { get; set; }
        public ViewportlatlngViewModel viewport { get; set; }
    }

    public class ResultlatlngViewModel
    {
        public List<AddressComponentlatlngViewModel> address_components { get; set; }
        public string formatted_address { get; set; }
        public Geometry geometry { get; set; }
        public string place_id { get; set; }
        public List<string> types { get; set; }
    }
    //latitude and longitude
    public class RootObjectlatlngViewModel
    {
        public List<ResultlatlngViewModel> results { get; set; }
        public string status { get; set; }
    }
}
