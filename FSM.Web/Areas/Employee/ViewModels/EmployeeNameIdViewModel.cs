using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class EmployeeNameIdViewModel
    {
       
            public Guid EmployeeId { get; set; }
            public string UserName { get; set; }
            public Nullable<Guid> FromId { get; set; }
        public List<EmployeeNameDetail> employeeDetail { get; set; }
     }
        public class EmployeeNameDetail
    {
            public Guid EmployeeId { get; set; }

            public string UserName { get; set; }

        }
}