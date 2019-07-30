using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Customer.ViewModels
{
    public class CustomerTabPanelViewModel
    {
        public string CustomerGeneralInfoId { get; set; }
        public string ContactId { get; set; }
        public string SiteDetailId { get; set; }
        public string BillingAddressId { get; set; }
        public string ResidenceDetailId { get; set; }
        public string ConditionReportId { get; set; }
        public string CustomerContactId { get; set; }
        public string DocumentId { get; set; }
        public string ActiveTab { get; set; }
        public string Success { get; set; }
        public string PageNum { get; set; }
        public string SiteCount { get; set; }
        public string BillingCount { get; set; }
        public string CustomerContactsCount { get; set; }
        public string SitesDocumentCount { get; set; }
        public string CustomerName { get; set; }
        public string JobId { get; set; }
    }
}