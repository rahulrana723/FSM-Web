using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class PurchaseDataViewModel
    {
        public string StockId { get; set; }
        public string StockLabel { get; set; }
        public string UnitMeasure { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }
        public string Cost { get; set; }
        public string Description { get; set; }
        public string Supplierid { get; set; }
        public string AvailableQuatity { get; set; }
        public string purchaseId { get; set; }
    }
}