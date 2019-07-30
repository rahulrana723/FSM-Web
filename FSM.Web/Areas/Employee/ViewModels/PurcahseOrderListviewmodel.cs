using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class PurcahseOrderListviewmodel
    {
        public IEnumerable<PurchaseOrderByStockViewModel> Purchaseorderviewmodel { get; set; }
        public PurchaseorderSearchViewmodel Purchasesearchorderviewmodel { get; set; }
    }
}