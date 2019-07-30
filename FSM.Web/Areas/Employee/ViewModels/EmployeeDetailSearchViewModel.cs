using FSM.Web.FSMConstant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class EmployeeDetailSearchViewModel
    {
        public string FirstName { get; set; }
        public List<string> GetUserRoles { get; set; }
        public int PageSize { get; set; }
        public string Role { get; set; }

    }
}