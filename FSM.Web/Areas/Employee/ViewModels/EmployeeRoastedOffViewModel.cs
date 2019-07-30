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
    public class EmployeeRoastedOffViewModel
    {
        public Guid ID { get; set; }
        [DisplayName("Select OTRW")]
        public string OTRWId { get; set; }
        public IEnumerable<SelectListItem> OTRWList { get; set; }
        [DisplayName("Select Day")]
        public Nullable<Constant.EmployeeRoastdDays> DayId { get; set; }
        [DisplayName("Select Weeks")]
        public List<int?> WeekId { get; set; }
        public List<SelectListItem> Weeks { get; set; }
        [DisplayName("Start Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MMM yyyy}")]
        public DateTime? StartDate { get; set; }

        [DisplayName("End Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MMM yyyy}")]
        public DateTime? EndDate { get; set; }
        public string RoastedOnOff { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
    }
}