using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class DisplayInvoicePaymentsViewModel
    {
        public string Id { get; set; }
        public string InvoiceId { get; set; }
        public string PaymentDate { get; set; }
        public string PaymentAmount { get; set; }
        public Nullable<int> PaymentMethod { get; set; }
        public string Reference { get; set; }
        public string PaymentNote { get; set; }
    }
}