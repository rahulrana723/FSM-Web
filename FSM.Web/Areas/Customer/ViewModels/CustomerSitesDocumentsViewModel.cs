using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Customer.ViewModels
{
    public class CustomerSitesDocumentsViewModel
    {
        public Guid DocumentId { get; set; }
        public Guid CustomerGeneralInfoId { get; set; }
        public Guid SiteId { get; set; }
        public string DocumentName { get; set; }
        public string DocType { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public string ModifiedDate { get; set; }
        public string PageNum { get; set; }
    }
    
}