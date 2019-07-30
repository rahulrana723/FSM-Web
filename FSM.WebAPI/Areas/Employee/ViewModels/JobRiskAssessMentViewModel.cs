using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class JobRiskAssessMentViewModel
    {
        public Guid Id { get; set; }
        public Guid JobId { get; set; }
        public Nullable<bool> SiteInspected { get; set; }
        public Nullable<bool> IssuedPPE { get; set; }
        public Nullable<bool> ReviewedPublicSafety { get; set; }
        public Nullable<bool> ElectricalRisk { get; set; }
        public Nullable<bool> SafetySystemAvailable { get; set; }
        public Nullable<bool> WeatherConsidered { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
    }
}