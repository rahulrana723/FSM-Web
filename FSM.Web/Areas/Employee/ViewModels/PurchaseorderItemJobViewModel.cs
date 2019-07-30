using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class PurchaseorderItemJobViewModel
    {
        public Guid ID { get; set; }
        public Nullable<Guid> PurchaseOrderID { get; set; }
        public Nullable<Guid> StockID { get; set; }
        public string UnitOfMeasure { get; set; }
        public string PurchaseItem { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public List<StockJobList> StockJoblist { get; set; }
    }
    public class StockJobList
    {
        public Guid StockId { get; set; }
        public string StockName { get; set; }
    }
}