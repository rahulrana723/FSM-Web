using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class GetJobDetailViewModel
    {
        public Nullable<Guid> Id { get; set; }
        public Nullable<int> JobId { get; set; }
        public Nullable<int> JobNo { get; set; }
        public Nullable<int> JobType { get; set; }
        public Nullable<int> JobStatus { get; set; }
        public string OTRWjobNotes { get; set; }
        public string OperationNotes { get; set; }
        public string CustomerLastName { get; set; }
        public string TradingName { get; set; }
        public Nullable<int> CustomerType { get; set; }
        public Nullable<int> LeadType { get; set; }
        public Nullable<int> Terms { get; set; }
        public string CustomerNotes { get; set; }
        public string JobAddress { get; set; }
        public string JobNotes { get; set; }
        public Guid SiteDetailId { get; set; }
        public Nullable<double> Latitude { get; set; }
        public Nullable<double> Longitude { get; set; }
        public string TimeSpent { get; set; }
        public string SiteFileName { get; set; }
        public Nullable<int> PrefTimeOfDay { get; set; }
        public Nullable<int> Contracted { get; set; }
        public string ScheduledPrice { get; set; }
        public string StrataPlan { get; set; }
        public string Notes { get; set; }
        public Nullable<int> IsRunning { get; set; }
        public string Reason { get; set; }
        public Nullable<TimeSpan> StartTime { get; set; }
        public string JobDate { get; set; }
        public string HasInvoice { get; set; }
        public Nullable<Guid> ConditionDetailId { get; set; }
        public Nullable<int> RoofTilesSheets { get; set; }
        public Nullable<int> BargeCappings { get; set; }
        public Nullable<int> RidgeCappings { get; set; }
        public Nullable<int> Valleys { get; set; }
        public Nullable<int> Flashings { get; set; }
        public Nullable<int> Gutters { get; set; }
        public Nullable<int> DownPipes { get; set; }
        public string ConditionNote { get; set; }
        public Nullable<Guid> ResidenceDetailId { get; set; }
        public Nullable<int> TypeOfResidence { get; set; }
        public string NoBldgs { get; set; }
        public Nullable<int> Height { get; set; }
        public Nullable<int> Pitch { get; set; }
        public Nullable<int> RoofType { get; set; }
        public Nullable<int> GutterGaurd { get; set; }
        public Nullable<int> SRASinstalled { get; set; }
        public string ResidenceUnit { get; set; }
        public Nullable<bool> NotWet { get; set; }
        public Nullable<bool> NeedTwoPPL { get; set; }
        public Nullable<int> ConditionOfRoof { get; set; }
    }
}