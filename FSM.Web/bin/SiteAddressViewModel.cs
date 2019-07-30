using System;
using FSM.Web.FSMConstant;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Customer.ViewModels
{
    public class SiteAddressViewModel
    {
        public string Street { get; set; }
        public string StreetName { get; set; }
        public Nullable<Constant.HomeAddressStreetType> StreetType { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public Nullable<int> PostalCode { get; set; }
    }
}