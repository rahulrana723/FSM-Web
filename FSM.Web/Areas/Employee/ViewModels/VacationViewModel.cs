using FSM.Core.Entities;
using FSM.Web.FSMConstant;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class VacationViewModel
    {
        public Guid Id { get; set; }
        public Nullable<Guid> EmployeeId { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("Start Date")]
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> OldStartDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("End Date")]
        public Nullable<DateTime> EndDate { get; set; }
        public Nullable<DateTime> OldEndDate { get; set; }
        public Nullable<int> Hours { get; set; }
        public string Reason { get; set; }
        public Constant.VacationType Status { get; set; }
        public string DisplayStatus { get; set; }
        [DisplayName("Leave Type")]
        public Nullable<Constant.EmployeeLeaveType> LeaveType { get; set; }
        public string DisplayLeaveType { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public int EmpId { get; set; }
        public string IsVisible { get; set; }
        public List<SelectListItem> OTRWList { get; set; }
        public virtual EmployeeDetail EmployeeDetail { get; set; }
    }
}