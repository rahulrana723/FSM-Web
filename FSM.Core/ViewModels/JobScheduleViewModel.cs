using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
    public class JobScheduleViewModel
    {
        public string jobNo { get; set; }
        public Guid? jobid { get; set; }
        public Guid? AssignTo { get; set; }
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
        public Nullable<int> JobNumber { get; set; }
        public string CustomerLastName { get; set; }
        public Nullable<int> JobType { get; set; }
        public Nullable<int> WorkType { get; set; }
        public Nullable<int> JobStatus { get; set; }
        public string Suburb { get; set; }
        public string JobNotes { get; set; }
        public Nullable<int> JobNoVal { get; set; }
        public string StrataNumber { get; set; }
        public string StrataPlan { get; set; }
        public Nullable<int> PreferTime { get; set; }
        public double EstimatedHours { get; set; }
        public Nullable<double> EstimatedHrsPerUser { get; set; }
        public Guid? ResourceId { get; set; }
        public string Address { get; set; }
        public Nullable<bool> WetRequiredType { get; set; }
        public Nullable<int> StoreysType { get; set; }
        public Nullable<int> OTRWAssignCount { get; set; }

    }
}
