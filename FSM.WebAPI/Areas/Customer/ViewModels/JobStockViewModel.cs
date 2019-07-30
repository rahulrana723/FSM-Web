using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Customer.ViewModels
{
    public class JobStockViewModel
    {
        public Guid Id { get; set; }
        public Nullable<Guid> UserId { get; set; }
        public Guid StockId { get; set; }
        public string UnitMeasure { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Guid JobId { get; set; }
        public string AssignedFrom { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
    }
}