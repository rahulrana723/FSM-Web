using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class InvoiceTabPanelViewModel
    {
        public string InvoiceId { get; set; }
        public Guid JobId { get; set; }
        public Nullable<int> JobNo { get; set; }
        public Nullable<int> JobType { get; set; }
        public string CustomerGeneralInfoId { get; set; }
        public string SiteDocumentsCount { get; set; }
        public string SiteContactId { get; set; }
        public string CustomerSiteDetailId { get; set; }
        public string BillingAddressId { get; set; }
        public string DocumentId { get; set; }
        public string InvoiceType { get; set; }
        public Nullable<int> InvoiceNo { get; set; }
        public string ActiveTab { get; set; }
        public string Success { get; set; }
        public string PageNum { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
    }
}