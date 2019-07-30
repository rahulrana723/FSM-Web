using FSM.Core.ViewModels;
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
    public class TimeSheetReportDataViewModel
    {
        public IEnumerable<UserTimeSheetViewModel> UserTimeSheetList { get; set; }
        public IEnumerable<TimeSheetTotal> SheetTotalHrs { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("Start Date")]
        public Nullable<DateTime> JobStartDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("End Date")]
        public Nullable<DateTime> JobEndDate { get; set; }
        public List<SelectListItem> Users { get; set; }
        public Guid UserId { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm\\:ss}")]
        public string Job { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm\\:ss}")]
        public string Lunch { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm\\:ss}")]
        public string Personal { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm\\:ss}")]
        public string Travelling { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm\\:ss}")]

        public Constant.ReportType ReportType { get; set; }
        public string TotalHrs { get; set; }
        public int PageSize { get; set; }
        public string UserName { get; set; }
        public string Keyword { get; set; }
    }
}