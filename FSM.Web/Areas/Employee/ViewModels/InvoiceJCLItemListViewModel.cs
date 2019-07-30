using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class InvoiceJCLItemListViewModel
    {
        public Guid ID { get; set; }
        public Guid JCLId { get; set; }
        public Guid JCLItemID { get; set; }
        public Guid JobId { get; set; }
        public Guid ProductStyleID { get; set; }
        public Nullable<Guid> ColorId { get; set; }
        public Nullable<Guid> SizeId { get; set; }
        public Nullable<Guid> ProductId { get; set; }
        public string ColorName { get; set; }
        public string Size { get; set; }
        public string ProductName { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
    }
}