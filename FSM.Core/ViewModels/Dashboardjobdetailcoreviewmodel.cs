using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
    public class Dashboardjobdetailcoreviewmodel
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public Nullable<Double> Latitude { get; set; }
        public Nullable<Double> Longitude { get; set; }
        public Nullable<Guid> CustomerGeneralInfoId { get; set; }
        public Nullable<int> JobId { get; set; }
        public Nullable<int> JobNo { get; set; }
        public Nullable<int> JobType { get; set; }
        public Nullable<int> WorkType { get; set; }
        public string JobTypeName { get; set; }
        public string OTRWjobNotes { get; set; }
        public Nullable<int> OTRWRequired { get; set; }
        public Nullable<int> OTRWAssignCount { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<DateTime> DateBooked { get; set; }
        public Nullable<int> PreferTime { get; set; }
        public Nullable<int> EstimatedHours { get; set; }
        public string JobNotes { get; set; }
        public string StreetName { get; set; }
        public string OperationNotes { get; set; }
        public Nullable<Guid> AssignTo { get; set; }
        public Nullable<Guid> BookedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public string SiteId1 { get; set; }
        public Nullable<Guid> SiteId { get; set; }
        public string BookedByName { get; set; }
        public string Street { get; set; }
        public Nullable<int> StreetType { get; set; }
        public string State { get; set; }
        public int  PostalCode { get; set; }
        public string suburb { get; set; }
        public string Address { get; set; }
        public string ContactName { get; set; }
        public string ContactNo { get; set; }
        public string StrataNumber { get; set; }
        public string StrataPlan { get; set; }
        public Nullable<double> EstimatedHrsPerUser { get; set; }
        public string SiteNotes { get; set; }
        public string CustomerNotes { get; set; }
        public Nullable<bool> WetRequiredType { get; set; }
        public Nullable<int> StoreysType { get; set; }
    }

    public class WorkingHoursModel
    {
        public double? HoursAvailable { get; set; }
        public int? HoursBooked { get; set; }
        public double? RoofHoursAvailable { get; set; }
        public double? CleaningHoursAvailable { get; set; }
    }
}
