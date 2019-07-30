using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class GetBillingListViewModel
    {
        public Guid BillingAddressId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNo { get; set; }
        public string LandlineNo { get; set; }
        public string AlternateNo { get; set; }
        public string EmailId { get; set; }

        public string ContactPosition { get; set; }
        public string StrataPlan { get; set; }
        public string RealEstate { get; set; }
        public string BillingNotes { get; set; }
        public string BillingAddress { get; set; }
    }
}