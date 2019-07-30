using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSM.WebAPI.Areas.Customer.ViewModels
{
    public class CustomerListViewModel
    {
        public Guid CustomerGeneralInfoId { get; set; }
        public int CTId { get; set; }
        public string CustomerLastName { get; set; }
        public Guid SiteDetailId { get; set; }
        public string Address { get; set; }
        public string Unit { get; set; }
        public string Street { get; set; }
        public string StreetName { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public int PostalCode { get; set; } 
        public string ContactName { get; set; }
        public string PhoneNo { get; set; }
        public string EmailAddress { get; set; }
        public string CustomerNotes { get; set; }
        public Guid ContactId { get; set; }
        public Nullable<double> Latitude { get; set; }
        public Nullable<double> Longitude { get; set; }
        public bool IsDelete { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        
    }
}