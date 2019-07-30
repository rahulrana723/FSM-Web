using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Common
{
    public class MyGeoCodeResponse
    {
        public string status { get; set; }
        public results[] results { get; set; }
    }
    public class results
    {
        public string formatted_address { get; set; }
        public _geometry geometry { get; set; }
        public string[] types { get; set; }
        public _address_component[] address_components { get; set; }
    }

    public class _geometry
    {
        public string location_type { get; set; }
        public _location location { get; set; }
        public _bounds bounds { get; set; }
    }

    public class _location
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }

    public class _bounds
    {
        public _northeast northeast { get; set; }
        public _southwest southwest { get; set; }
    }

    public class _northeast
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }

    public class _southwest
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }

    public class _address_component
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public string[] types { get; set; }
    }
}