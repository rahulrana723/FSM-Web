using FSM.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class PurchaseOrderByJobViewModel
    {
        public Guid ID { get; set; }
        public Nullable<Guid> SupplierID { get; set; }
        public string SupplierName { get; set; }
        public Nullable<int> PurchaseOrderNo { get; set; }
        public string Description { get; set; }
        public Decimal Cost { get; set; }
        public Nullable<Guid> JobID { get; set; }
        public Nullable<Guid> InvoiceId { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public List<PurchaseOrderItemsViewModel> lstPurchaseOrderItem { get; set; }
        public ICollection<PurchaseorderItemJob> PurchaseorderItemJobs { get; set; }
        
        public string PONumber { get; set; }
    }

}