using FSM.Core.Entities;
using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Customer.ViewModels
{
    public class CustomerSiteDocumentsListViewModel
    {
        public CustomerSitesDocuments CustomerSiteDocuments { get; set; }
        public List<CustomerSitesDocuments> CustomerSiteDocumentsViewModelList { get; set; }
        public CustomerSiteDetailViewModel CustomerSiteDetailViewModel { get; set; }
        public CustomerSiteCountViewModel SiteCountviewModel { get; set; }
        public IEnumerable<CustomerSitesDocumentsViewModelCore> CustomerSiteDocumentsCoreViewModelList { get; set; }
    }
}