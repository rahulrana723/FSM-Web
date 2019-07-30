using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
    public class GetQuickViewJobDataCoreViewModel
    {
        public Guid JobId { get; set; }
        public string CustomerNotes { get; set; }
        public string OperationNotes { get; set; }
        public string JobNotes { get; set; }
        public Nullable<int> JobType { get; set; }
        public Nullable<decimal> InvoicePrice { get; set; }
        public Nullable<decimal> Paid { get; set; }
        public Guid? ApprovedBy { get; set; }
        public string  ApprovedByName { get; set; }
        public string TimeTaken { get; set; }
        public string DisplayType { get; set; }
    }
}
