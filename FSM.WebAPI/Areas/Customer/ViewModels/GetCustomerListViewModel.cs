using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Customer.ViewModels
{
    public class GetCustomerListViewModel
    {
        public Guid CustomerGeneralInfoId { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerNotes { get; set; }
        public Guid SiteDetailId { get; set; }
        public string Address { get; set; }
        public string ContactName { get; set; }
        public string PhoneNo { get; set; }
        public string EmailAddress { get; set; }
        public Guid ContactId { get; set; }
        public string Unit { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}