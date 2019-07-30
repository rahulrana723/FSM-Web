using FSM.Core.Entities;
using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class EmployeeDetailListViewModel
    {
        public IEnumerable<EmpDetailListViewModel> EmployeeDetailList { get; set; }
        public EmployeeDetailSearchViewModel EmployeeDetailInfo { get; set; }
    }
}