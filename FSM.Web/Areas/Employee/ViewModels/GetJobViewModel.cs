using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class GetJobViewModel
    {

        public Guid EmployeeJobId { get; set; }
        public Nullable<int> JobId { get; set; }
        public Nullable<int> JobNo { get; set; }
        public Nullable<int> Id { get; set; }
        public string Description { get; set; }
        public Guid SupplierId { get; set; }
        public List<SelectListItem> SupplierList { get; set; }
        public List<EmployeeJobDetail> employeeJobDetail { get; set; }
        public List<System.Web.Mvc.SelectListItem> JobDetailsList { get; set; }
        public int PurchaseOrderNo { get; set; }
    }
    public class EmployeeJobDetail
    {
        public Guid EmployeeJobId { get; set; }

        public Nullable<int> JobId { get; set; }
        public Nullable<int> JobNo { get; set; }
        public string Description { get; set; }
        public List<System.Web.Mvc.SelectListItem> JobDetailsList { get; set; }

    }
}