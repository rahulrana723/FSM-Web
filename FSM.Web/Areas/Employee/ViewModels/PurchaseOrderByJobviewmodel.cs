using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class PurchaseOrderByJobviewmodel
    {
        public Guid ID { get; set; }
        public Nullable<Guid> SupplierID { get; set; }
        public string Description { get; set; }
        public Decimal Cost { get; set; }
        public decimal? GST { get; set; }
        public decimal? PriceWithGst { get; set; }
        public Nullable<Guid> JobID { get; set; }
        public Nullable<Guid> InvoiceId { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<int> PurchaseOrderNo { get; set; }
        public string SupplierName { get; set; }
        public List<SupplierJobItem> SupplierJobList { get; set; }
        public Nullable<int> jobidnumeric { get; set; }
        public Nullable<int> JobNo { get; set; }
        public Nullable<int> InvoiceNo { get; set; }
        public string PurchaseOrderNoformated { get; set; }
        public Nullable<bool> IsApprove { get; set; }
        public string ApproveStatus { get; set; }
        public Nullable<bool> IsDelete { get; set; }
    }
    public class SupplierJobItem
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
    }
}