using FSM.Web.FSMConstant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class InvoiceSearchViewModel
    {
        public int PageSize { get; set; }
        public string searchkeyword { get; set; }
        public Nullable<Constant.CustomerType> CustomerType { get; set; }
        public Nullable<Guid> JobId { get; set; }
        public Nullable<int> JobNo { get; set; } 
        public Constant.InvoiceSentStatus SentStatus { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public Nullable<Guid> CustomerGeneralInfoId { get; set; }
        public int? TotalCount { get; set; }
    }

    public class MaterialSearchViewModel
    {
        public int PageSize { get; set; }
        public string searchkeyword { get; set; }

    }
}