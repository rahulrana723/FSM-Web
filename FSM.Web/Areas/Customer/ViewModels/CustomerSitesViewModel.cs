using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSM.Web.Areas.Customer.ViewModels
{
    public class CustomerSitesViewModel
    {
        public CustomerSiteDetailViewModel CustomerSiteDetailViewModel { get; set; }
        public CustomerResidenceDetailViewModel CustomerResidenceDetailViewModel { get; set; }
        public CustomerConditionReportViewModel CustomerConditionReportViewModel { get; set; }
        public IEnumerable<DisplaySitesViewModel> DisplaySitesViewModel { get; set; }
        public Guid CustomerGeneralInfoId { get; set; }
        public Guid SiteDetailId { get; set; }
        public Guid ConditionReportId { get; set; }
        public Guid ResidenceDetailId { get; set; }
        public List<SelectListItem> ContactList { get; set; }
        public List<SelectListItem> StrataManagerList { get; set; }
        public List<SelectListItem> RealEstateList { get; set; }
        public int PageSize { get; set; }
        public int SiteCount { get; set; }
        public string HideAddCustomer { get; set; }
        public string UserName { get; set; }
       
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
}
}