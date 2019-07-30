using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class PurchaseOrderITemByStockViewModel
    {
        public Guid ID { get; set; }
        public Guid StockID { get; set; }
        public Nullable<Guid> PurchaseOrderID { get; set; }
        public string UnitOfMeasure { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<Decimal> Price { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public List<StockList> Stocklist { get; set; }
    }
    public class StockList
    {
        public Guid StockId { get; set; }
        public string StockName { get; set; }
    }
}