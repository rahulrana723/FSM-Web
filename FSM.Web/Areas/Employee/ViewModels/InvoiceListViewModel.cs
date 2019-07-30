using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class InvoiceListViewModel
    {
        public IEnumerable<CreateInvoiceViewModel> createInvoiceViewModel { get; set; }
        public InvoiceSearchViewModel invoiceSearchViewModel { get; set; }
    }

    public class ApprovedQuoteListViewModel
    {
        public IEnumerable<MaterialViewModel> MaterialInvoiceViewModel { get; set; }
        public string searchkeyword { get; set; }
        public int PageSize { get; set; }
    }
}