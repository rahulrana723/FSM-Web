using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
    public class PurchaserOrderByJobCoreViewModel
    {
        public Guid ID { get; set; }
        public Nullable<int> PurchaseOrderNo { get; set; }
        public Nullable<Guid> SupplierID { get; set; }
        public string Description { get; set; }
        public Decimal Cost { get; set; }
        public Nullable<Guid> JobID { get; set; }
        public Nullable<Guid> InvoiceId { get; set; }
        public Nullable<int> InvoiceNo { get; set; }
        public string PurchaseOrderNoformated { get; set; }
        public string SupplierName { get; set; }
        public Nullable<bool> IsApprove { get; set; }
        public Nullable<int> jobidnumeric { get; set; }
        public Nullable<int> JobNo { get; set; }
    }
}
