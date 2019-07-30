using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class UserTimeSheetViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid JobId { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime JobDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm\\:ss}")]
        [RegularExpression(pattern: "([0-1]?\\d|2[0-3]):([0-5]?\\d):([0-5]?\\d)")]
        [DisplayName("Start Time")]
        public TimeSpan StartTime { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm\\:ss}")]
        [RegularExpression(pattern: "([0-1]?\\d|2[0-3]):([0-5]?\\d):([0-5]?\\d)")]
        [DisplayName("End Time")]
        public TimeSpan EndTime { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm\\:ss}")]
        public TimeSpan TimeSpent { get; set; }
        public string Reason { get; set; }
        [DisplayName("Reason To Update")]
        public string ReasonToUpdate { get; set; }
        public int IsRunning { get; set; }
        public string Site { get; set; }
        public string UserName { get; set; }
        public string CustomerLastName { get; set; }
        public string pagenum { get; set; }
        public string jobstartdateSearch { get; set; }
        public string jobenddateSearch { get; set; }
        public string useridSearch { get; set; }
        public string Keyword { get; set; }
        public int Job { get; set; }
        public Nullable<int> JobNo { get; set; }
        public string PageSize { get; set; }
        public string TotalTimeSpent { get; set; }
        public string JobTimeSpent { get; set; }
        public string LunchTimeSpent { get; set; }
        public string PersonnalTimeSpent { get; set; }
        public List<EmployeeJobTimeSheetDetail> employeeJobTimeSheetDetail { get; set; }
        
    }
    public class EmployeeJobTimeSheetDetail
    {
        public Guid EmployeeJobId { get; set; }
        public Nullable<int> JobNo { get; set; }
        public string Description { get; set; }

    }
}