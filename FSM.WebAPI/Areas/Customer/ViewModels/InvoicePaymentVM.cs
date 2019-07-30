using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Customer.ViewModels
{
    public class InvoicePaymentVM
    {
        public Guid Id { get; set; }
        public Guid InvoiceId { get; set; }
        public Nullable<DateTime> PaymentDate { get; set; }
        public Nullable<Decimal> PaymentAmount { get; set; }
        public Nullable<int> PaymentMethod { get; set; }
        public string Reference { get; set; }
        public string PaymentNote { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
    }
}