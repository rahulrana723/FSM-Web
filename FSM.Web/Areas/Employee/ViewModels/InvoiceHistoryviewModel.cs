using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class InvoiceHistoryviewModel
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public Nullable<Guid> CustomerGeneralInfoId { get; set; }
        public Nullable<int> JobId { get; set; }
        public Nullable<int> JobNo { get; set; }
        public Nullable<int> InvoiceNo { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string AssignUser { get; set; }
        public Nullable<DateTime> CompletedDate { get; set; }
        public Guid SiteId { get; set; }
        public string SiteAddress { get; set; }
        public string CreatedBy { get; set; }
    }
}