using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class ConditionDetailViewModel
    {
        public Guid ConditionReportId { get; set; }
        public Guid SiteDetailId { get; set; }
        public Nullable<int> RoofTilesSheets { get; set; }
        public Nullable<int> BargeCappings { get; set; }
        public Nullable<int> RidgeCappings { get; set; }
        public Nullable<int> Valleys { get; set; }
        public Nullable<int> Flashings { get; set; }
        public Nullable<int> Gutters { get; set; }
        public Nullable<int> DownPipes { get; set; }
        public string ConditionNote { get; set; }
        public Nullable<int> ConditionOfRoof { get; set; }
    }
}