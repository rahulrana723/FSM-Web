using FSM.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Customer.ViewModels
{
    public class CustomerListViewModel
    {
        public IEnumerable<CustomerGeneralInfoViewModel> CustomerGeneralInfoList { get; set; }
        public CustomerSearchViewModel CustomerGeneralInfo { get; set; }
        public CustomerGeneralInfoViewModel CustomerGeneralInfoWizard { get; set; }
        public CustomerSiteDetailViewModel customerSitesViewModelWizard { get; set; }
        public CustomerContactsViewModel CustomerContactsViewModelWizard { get; set; }
        public CustomerSitesGeneralInfoViewModel Customerwithsites { get; set; }
    }
}