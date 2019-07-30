using FSM.Web.FSMConstant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FSM.Core.Entities;

namespace FSM.Web.Areas.Admin.ViewModels
{
    public class DashboardjobInfoViewModel
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public string StreetName { get; set; }
        public string JobTypeName { get; set; }
        public Nullable<Double> Latitude { get; set; }
        public Nullable<Double> Longitude { get; set; }
        public Nullable<Guid> CustomerGeneralInfoId { get; set; }
        public Nullable<int> JobId { get; set; }
        public Nullable<int> JobNo { get; set; }
        public Nullable<Constant.JobType> JobType { get; set; }
        public Nullable<Constant.AccountJobType> AccountJobType { get; set; }
        public Nullable<Constant.JobStatusForDashboard> Status { get; set; }
        public Nullable<DateTime> DateBooked { get; set; }
        public Nullable<Constant.PrefTimeOfDay> PreferTime { get; set; }
        public Nullable<int> EstimatedHours { get; set; }
        public string JobNotes { get; set; }
        public string OperationNotes { get; set; }
        public Nullable<Guid> AssignTo { get; set; }
        public Nullable<Guid> BookedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<Guid> SiteId { get; set; }
        public HttpPostedFileBase attachment { get; set; }
        public string BookedByName { get; set; }
        public string StatusText { get; set; }
        public Nullable<Constant.JobType> JobTypes { get; set; }
        public string WorkTypeName { get; set; }
        public Nullable<Constant.WorkType> WorkType { get; set; }
        public string PreferTimeText { get; set; }
        public string Datetimetext { get; set; }
        public string Street { get; set; }
        public Nullable<int> StreetType { get; set; }
        public string State { get; set; }
        public int PostalCode { get; set; }
        public string jobAddress { get; set; }
        public string StreetTypeName { get; set; }
        public string suburb { get; set; }
        public string Address { get; set; }
        public string ContactName { get; set; }
        public string ContactNo { get; set; }
        public string title { get; set; }
        public string StrataNumber { get; set; }
        public Nullable<int> OTRWRequired { get; set; }
        public Nullable<int> OTRWAssignCount { get; set; }
        public string DisplayOTRWAssignCount { get; set; }
        public Nullable<double> EstimatedHrsPerUser { get; set; }
        public string SiteFileName { get; set; }
        public string SiteNotes { get; set; }
        public string CustomerNotes { get; set; }

        public Nullable<bool> WetRequiredType { get; set; }
        public string WetRequiredName { get; set; }
        public Nullable<int> StoreysType { get; set; }
        public string StoreysName { get; set; }

        public string StrataPlan { get; set; }
    }
   
}