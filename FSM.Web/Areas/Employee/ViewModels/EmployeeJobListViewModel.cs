using FSM.Web.Areas.Customer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class EmployeeJobListViewModel
    {
        public IEnumerable<EmployeeJobsViewModel> EmployeeJoblist { get; set; }
        public EmployeejobSearchViewModel Employeejobsearchmodel { get; set; }
        public CustomerSiteCountViewModel SiteCountviewModel { get; set; }
        public string CustomerName { get; set; }
    }
}