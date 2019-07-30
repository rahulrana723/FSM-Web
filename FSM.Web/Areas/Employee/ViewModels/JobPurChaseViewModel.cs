using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class JobPurChaseViewModel
    {
        public PurchaseorderItemJobViewModel PurchaseOrderITemByJobViewModel { get; set; }
        public PurchaseOrderByJobviewmodel PurchaseOrderByJobViewModel { get; set; }
        public GetJobViewModel getjobviewmodel { get; set; }
        public EmployeeJobDetail employeejobdetail { get; set; }
        public IEnumerable<PurchaseDatajobviewModel> PurchaseDataListviewModel { get; set; }
        
        public string UserRole { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public string UserName { get; set; }
    }
}