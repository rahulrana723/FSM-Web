using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
    public class EmployeeJobVieweModel
    {
        public Guid Id { get; set; }
        public Guid EmployeeJobId { get; set; }
        public string CustomerName { get; set; }
        public Nullable<Guid> CustomerGeneralInfoId { get; set; }
        public Nullable<int> JobId { get; set; }
        public Nullable<int> JobNo { get; set; }
        public Nullable<int> JobType { get; set; }
        public Nullable<int> WorkType { get; set; }
        public string OTRWjobNotes { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<DateTime> DateBooked { get; set; }
        public Nullable<int> PreferTime { get; set; }
        public Nullable<int> EstimatedHours { get; set; }
        public string JobNotes { get; set; }
        public string OperationNotes { get; set; }
        public Nullable<Guid> AssignTo { get; set; }
        public Nullable<Guid> BookedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Guid SiteId { get; set; }
        public string BookedByName { get; set; }
        public string CustomerLastName { get; set; }
        public string StreetName { get; set; }
        public string Suburb { get; set; }
        public Nullable<int> InvoiceStatus { get; set; }
        public Nullable<Guid> InvoiceId { get; set; }
        public Nullable<int> Contracted { get; set; }
        public string StrataPlan { get; set; }
        public string StrataNumber { get; set; }
        public string SiteAddress { get; set; }
        public string AssignUser { get; set; }
        public string ContactName { get; set; }
        public bool IsDelete { get; set; }
        public string SiteFileName { get; set; }
        public string InvoiceDetails { get; set; }
        public int? TotalCount { get; set; }
    }
}
