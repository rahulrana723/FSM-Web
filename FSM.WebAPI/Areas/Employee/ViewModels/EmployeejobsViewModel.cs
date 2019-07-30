using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class EmployeejobsViewModel
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CustomerGeneralInfoId { get; set; }
        public Nullable<Guid> SiteId { get; set; }
        public string TimeSpent { get; set; }
        public Nullable<int> JobType { get; set; }
        public Nullable<int> Category { get; set; }
        public Nullable<int> OTRWRequired { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<Double> EstimatedTime { get; set; }
        public string StockRequired { get; set; }
        public Nullable<Double> TotalCost { get; set; }
        public string JobCategory { get; set; }
        public Nullable<DateTime> DateBooked { get; set; }
        public Nullable<int> PreferTime { get; set; }
        public Nullable<int> EstimatedHours { get; set; }
        public string JobNotes { get; set; }
        public string OTRWjobNotes { get; set; }
        public string OperationNotes { get; set; }
        public Nullable<Guid> AssignTo { get; set; }
        public Nullable<Guid> BookedBy { get; set; }
        public Nullable<DateTime> StartTime { get; set; }
        public Nullable<DateTime> EndTime { get; set; }
        public Nullable<Guid> LinkedJobId { get; set; }
        public Nullable<int> WorkType { get; set; }
    }
    public class Joblist
    {
        public Guid Id { get; set; }
        public int JobId { get; set; }
    }
}