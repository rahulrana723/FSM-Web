using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class GetEmployeeJobsViewModel
    {
        public Nullable<Guid> Id { get; set; }
        public Nullable<int> JobId { get; set; }
        public Nullable<int> JobNo { get; set; }
        public Nullable<DateTime> DateBooked { get; set; }
        public Nullable<DateTime> StartTime { get; set; }
        public string CustomerLastName { get; set; }
        public string JobAddress { get; set; }
        public string JobNotes { get; set; }
        public string OTRWjobNotes { get; set; }
        public Nullable<int> status { get; set; }
        public Nullable<int> JobType { get; set; }
        public Nullable<double> Latitude { get; set; }
        public Nullable<double> Longitude { get; set; }
    }
}

