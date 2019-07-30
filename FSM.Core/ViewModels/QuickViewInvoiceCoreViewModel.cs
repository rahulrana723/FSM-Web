using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
  public  class QuickViewInvoiceCoreViewModel
    {
        public Guid JobId { get; set; }
        public string CustomerNotes { get; set; }
        public string OperationNotes { get; set; }
        public string JobNotes { get; set; }
        public Nullable<int> JobType { get; set; }
        public Nullable<decimal> InvoicePrice { get; set; }
        public Nullable<decimal> Paid { get; set; }
        public decimal? Due { get; set; }
        public string ApprovedBy { get; set; }

        public string ApprovedByName { get; set; }
        public string TimeTaken { get; set; }
        public string DisplayType { get; set; }

        public string BillingContact { get; set; }
        public string BillingNotes{ get; set; }

        public string OTRWNotes { get; set; }

        public Boolean? Photos { get; set; }
        public Boolean? RequiredDocs { get; set; }
        public Boolean? IsPaid { get; set; }
        public Boolean? Stock { get; set; }
        public Boolean? Material { get; set; }
        public Boolean? IsApproved { get; set; }
        public Nullable<int> SentStatus { get; set; }
    }
}
