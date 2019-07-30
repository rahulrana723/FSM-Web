using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class LogSearchViewModel
    {
        public string Message { get; set; }
        public int PageSize { get; set; }
        public string searchkeyword { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
       
    }
}