using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class PurchaseOrderjobListviewModel
    {
        public IEnumerable<PurchaseOrderByJobviewmodel> PurchaseorderjobViewmodel { get; set; }
        public PurchaseOrderjobsearchviewModel Purchasejobsearchorderviewmodel { get; set; }
    }
}