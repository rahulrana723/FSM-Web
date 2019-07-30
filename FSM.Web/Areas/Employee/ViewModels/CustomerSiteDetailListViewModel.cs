using FSM.Web.Areas.Customer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class CustomerSiteDetailListViewModel
    {
        public CustomerSiteDetailViewModel CustomerSiteDetailmodel { get; set; }
        public CustomerResidenceDetailViewModel CustomerResidenceDetailmodel { get; set; }
        public CustomerConditionReportViewModel CustomerconditionDetailmodel { get; set; }
        public CustomerContactsViewModel Customercontactsmodel { get; set; }
        public IEnumerable<CustomerSitesDocumentsViewModel> CustomerSitedocumentviewmodel { get; set; }
    }
}