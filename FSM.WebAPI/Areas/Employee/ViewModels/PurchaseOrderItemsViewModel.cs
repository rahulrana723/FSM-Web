using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class PurchaseOrderItemsViewModel
    {
        public Guid ID { get; set; }
        public Nullable<Guid> PurchaseOrderID { get; set; }
        public string PurchaseItem { get; set; }
        public string UnitOfMeasure { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }

    }
}