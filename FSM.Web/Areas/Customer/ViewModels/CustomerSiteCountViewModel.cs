using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Customer.ViewModels
{
    public class CustomerSiteCountViewModel
    {
        public Guid SiteDetailId { get; set; }
        public string StreetName { get; set; }
        public List<SiteDetail> siteDetail { get; set; }
    }
    public class SiteDetail
    {
        public Guid SiteDetailId { get; set; }

        public string StreetName { get; set; }

        public string filecount { get; set; }
        public string SiteAddress { get; set; }
    }

}