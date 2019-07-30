using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class StockPurChaseViewModel
    {
        public PurchaseOrderITemByStockViewModel PurchaseOrderITemByStockViewModel { get; set; }
        public PurchaseOrderByStockViewModel PurchaseOrderByStockViewModel { get; set; }
    }
}