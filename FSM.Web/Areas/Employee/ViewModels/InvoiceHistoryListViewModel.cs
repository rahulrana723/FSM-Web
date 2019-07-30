using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class InvoiceHistoryListViewModel
    {
        public IEnumerable<InvoiceHistoryviewModel> invoiceHistoryViewModel { get; set; }
        public InvoiceSearchViewModel invoiceSearchViewModel { get; set; }
    }
}