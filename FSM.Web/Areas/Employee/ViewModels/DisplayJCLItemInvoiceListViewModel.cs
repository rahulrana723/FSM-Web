using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class DisplayJCLItemInvoiceListViewModel
    {
        public DisplayJCLItemInvoiceViewModel displayJCLItemInvoiceViewModel { get; set; }
        public IEnumerable<DisplayJCLItemInvoiceViewModel> DisplayJCLInvoiceList { get; set; }
    }
}