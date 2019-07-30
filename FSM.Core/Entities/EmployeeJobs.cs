using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public Nullable<int> JobNo { get; set; }
        [ForeignKey("CustomerSiteDetail")]
        //public Nullable<Guid> SiteId { get; set; }
        public Guid SiteId { get; set; }
        public Nullable<int> JobType { get; set; }
        public string TimeSpent { get; set; }
        public string OTRWjobNotes { get; set; }
        public Nullable<Double> EstimatedTime { get; set; }
        public string StockRequired { get; set; }
        public Nullable<Double> TotalCost { get; set; }
        public string JobCategory { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<DateTime> DateBooked { get; set; }
        public Nullable<int> PreferTime { get; set; }
        public Nullable<Double> EstimatedHours { get; set; }
        public Nullable<Double> EstimatedHrsPerUser { get; set; }
        public string JobNotes { get; set; }
        public string OperationNotes { get; set; }
        public Nullable<int> Category { get; set; }
        public Nullable<Guid> AssignTo { get; set; }
        public Nullable<int> SendJobEmail { get; set; }
        public Nullable<int> ReSendJobEmail { get; set; }
        public Nullable<int> WorkType { get; set; }
        public Nullable<int> OTRWRequired { get; set; }
        public Nullable<Guid> Supervisor { get; set; }
        public Nullable<Guid> BookedBy { get; set; }
        public Nullable<DateTime> StartTime { get; set; }
        public Nullable<DateTime> EndTime { get; set; }
        public Nullable<bool>  IsOverdue { get; set; }
        public string OverdueHours { get; set; }
        public Nullable<bool> IsApproved { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public virtual CustomerGeneralInfo CustomerGeneralInfo { get; set; }
        public virtual CustomerSiteDetail CustomerSiteDetail { get; set; }
        public bool IsDelete { get; set; }
        public string WorkOrderNumber { get; set; }
        public Nullable<int> NotificationType { get; set; }
        public Nullable<DateTime> ContractDueDate { get; set; }
        public Nullable<DateTime> ApprovedDate { get; set; }
    }
}
