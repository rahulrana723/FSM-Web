using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class LogListViewModel
    {
        public IEnumerable<LogViewModel> logDetailList { get; set; }
        public LogSearchViewModel logDetailInfo { get; set; }
    }
}