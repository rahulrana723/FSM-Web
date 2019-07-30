using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
    public class Jobs
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("CustomerGeneralInfo")]
        public Guid CustomerGeneralInfoId { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Nullable<int> JobId { get; set; }
        public Nullable<Guid> SiteId { get; set; }
        public Nullable<int> JobType { get; set; }
        public string TimeSpent { get; set; }
        public string OTRWjobNotes { get; set; }
        public Nullable<Double> EstimatedTime { get; set; }
        public string StockRequired { get; set; }
        public Nullable<Double> TotalCost { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<DateTime> DateBooked { get; set; }
        public Nullable<int> PreferTime { get; set; }
        public Nullable<int> EstimatedHours { get; set; }
        public string JobNotes { get; set; }
        public string OperationNotes { get; set; }
        public Nullable<Guid> AssignTo { get; set; }
        public Nullable<Guid> BookedBy { get; set; }
        public Nullable<DateTime> StartTime { get; set; }
        public Nullable<DateTime> EndTime { get; set; }
        public Nullable<DateTime> StartTravelTime { get; set; }
        public decimal TotalTravelTime { get; set; }
        public Nullable<DateTime> StartLunchTime { get; set; }
        public decimal TotalLunchTime { get; set; }
        public Nullable<DateTime> StartPersonalTime { get; set; }
        public decimal TotalPersonalTime { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public virtual CustomerGeneralInfo CustomerGeneralInfo { get; set; }
        public string TotalTravelTime { get; set; }
        public string TotalLunchTime { get; set; }
        public string TotalPersonalTime { get; set; }

    }
}
