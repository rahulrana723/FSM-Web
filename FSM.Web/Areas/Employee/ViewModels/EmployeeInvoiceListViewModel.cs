using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class EmployeeInvoiceListViewModel
    {
        public IEnumerable<CreateInvoiceViewModel> EmployeeInvoicelist { get; set; }
    }
}