using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static FSM.Web.FSMConstant.Constant;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class ReportsViewModel
    {
        [Required(ErrorMessage ="Please Select Report Type")]
        public TimesheetReportType ReportType { get; set; }
        public List<SelectListItem> EmployeeList { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public List<Nullable<Guid>> EmployeeIds { get; set; }
        public Nullable<Guid>EmployeeId { get; set; }
        //public List<Nullable<Guid>> JobIDs { get; set; }
        //public Nullable<Guid>jobid { get; set; }
        public Nullable<FSMConstant.Constant.OperationalReportType> Duration { get; set; }
        //public List<SelectListItem> Joblist { get; set; }
    }
   

}