using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Customer.ViewModels
{
    public class CustomerContactLogViewModel
    {
        [Key]
        public Guid CustomerContactId { get; set; }
        public Guid CustomerGeneralInfoId { get; set; }
        public Nullable<Guid> SiteId { get; set; }
        [DisplayName("Site Address")]
        public string SiteName { get; set; }
        [DisplayName("Site File Name")]
        public string SiteFileName { get; set; }
        [DisplayName("Customer Id")]
        public string CustomerId { get; set; }
        [DisplayName("Job Id")]
        public string JobId { get; set; }
        [DisplayName("Log Date")]
        public Nullable<DateTime> LogDate { get; set; }
        [DisplayName("ReContact Date")]
        public Nullable<DateTime> ReContactDate { get; set; }
        public string EnteredBy { get; set; }
        public string Type { get; set; }
        public string Note { get; set; }
        public string PageNum { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<int> ViewJobid { get; set; }
        public List<CustomerJobs> Customerjobs { get; set; }
    }

    public class CustomerJobs
    {
        public Nullable<Guid> CustJobId { get; set; }
        public string Jobtext { get; set; }
    }
}