using FSM.Web.FSMConstant;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class EmployeejobSearchViewModel
    {
        [DisplayName("Job Type")]
        public Constant.JobType JobType { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        [DisplayName("SOL")]
        public bool SOL { get; set; }
        [DisplayName("LNC")]
        public bool LNC { get; set; }
        [DisplayName("Unsent Inv")]
        public bool UnsentInv { get; set; }
        public int PageSize { get; set; }
        public string Keyword { get; set; }
        public string searchkeyword { get; set; }
        //public Constant.InvoiceStatus InvoiceStatus { get; set; }
        public string InvoiceStatus { get; set; }
        public bool Contracted { get; set; }
        public Guid CustomerGeneralInfoId { get; set; }
        public string CustomerInfoId { get; set; }
        public int? TotalCount { get; set; }
        public List<SelectListItem> CustomerList { get; set; }

    }
}