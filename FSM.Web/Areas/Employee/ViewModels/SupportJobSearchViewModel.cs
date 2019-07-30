using FSM.Web.FSMConstant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class SupportJobSearchViewModel
    {
        public int PageSize { get; set; }
        public string searchkeyword { get; set; }
        public Constant.JobType JobType { get; set; }
        public Nullable<Guid> SupportJobid { get; set; }
    }
}