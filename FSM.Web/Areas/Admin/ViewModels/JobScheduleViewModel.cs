using FSM.Web.FSMConstant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSM.Web.Areas.Admin.ViewModels
{
    public class JobScheduleViewModel
    {
        public string jobNo { get; set; }
        public Guid jobid { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public TimeSpan? TimeSpent { get; set; }
        public DateTime? DateBooked { get; set; }
        public string SiteName { get; set; }
        public string Employeename { get; set; }
        public Guid? EmployeeId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Sitpostalcode { get; set; }
        public DateTime? MinJobDate { get; set; }
        public DateTime? MaxJobDate { get; set; }
        public int JobNumber { get; set; }
        public int JobNoVal { get; set; }
        public string CustomerLastName { get; set; }
        public int JobType { get; set; }
        public int JobStatus { get; set; }
        public Constant.WorkType WorkType { get; set; }
        public string Suburb { get; set; }
        public string JobNotes { get; set; }
        public Nullable<Constant.PrefTimeOfDay> PreferTime { get; set; }
        public double EstimatedHrsPerUser { get; set; }
        public string StrataNumber { get; set; }
        public string StrataPlan { get; set; }
        //public List<SelectListItem> EmployeeList { get; set; }
        public List<Employeedetail> EmployeeList { get; set; }
        public List<EventDetail> EventDetail { get; set; }

        public string Address { get; set; }
        public Nullable<bool> WetRequiredType { get; set; }
        public string WetRequiredName { get; set; }
        public Nullable<int> StoreysType { get; set; }
        public string StoreysName { get; set; }
        public Nullable<int> OTRWAssignCount { get; set; }
        public string DisplayOTRWAssignCount { get; set; }
    }
    public class Employeedetail
    {
        public string title { get; set; }
        public string id { get; set; }
        public string eventColor { get; set; }
        
    }
    public class EventDetail
    {
        public string id { get; set; }
        public string resourceId { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string title { get; set; }
        public string title_EmployeeId { get; set; }
        public string JobId { get; set; }
        public string data_Job { get; set; }
        public string data_CustomerName { get; set; }
        public string data_JobType { get; set; }
        public string data_Status { get; set; }
        public string data_Date { get; set; }
        public string data_Suburb { get; set; }
        public string data_JobVal { get; set; }
        public string data_JobId { get; set; }
        public string data_SpanClass { get; set; }
        public string color { get; set; }
        public string className { get; set; }
        public double EstimatedHrsPerUser { get; set; }
    }
}
