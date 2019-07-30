using FSM.Web.Areas.Employee.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Admin.ViewModels
{
    public class DashboardViewModels
    {
        public IEnumerable<EmployeeJobsViewModel> EmployeeJoblist { get; set; }
    }
}