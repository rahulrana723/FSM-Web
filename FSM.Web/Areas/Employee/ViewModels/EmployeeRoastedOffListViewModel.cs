using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class EmployeeRoastedOffListViewModel
    {
       
        public EmployeeRoastedOffViewModel EmployeeRoastedOffViewModel { get; set; }
        public IEnumerable<RoastedOffCoreViewModel> RoastedOffCoreListViewModel { get; set; }
        public int PageSize { get; set; }
        public string SearchKeyword { get; set; }
    }
}