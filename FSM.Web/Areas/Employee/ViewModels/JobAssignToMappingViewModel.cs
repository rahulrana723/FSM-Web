using FSM.Web.FSMConstant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class JobAssignToMappingViewModel
    {
        public Guid Id { get; set; }
        public Guid JobId { get; set; }
        public Nullable<Guid> AssignTo { get; set; }
        public Constant.JobStatus Status { get; set; }
        public Nullable<DateTime> DateBooked { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> StartTime { get; set; }
        public Nullable<DateTime> EndTime { get; set; }
        public Nullable<Double> EstimatedHrsPerUser { get; set; }
        public bool IsDelete { get; set; }
        public string  OTRWNotes { get; set; }
    }
}