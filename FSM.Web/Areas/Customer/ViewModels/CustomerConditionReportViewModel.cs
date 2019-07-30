using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FSM.Web.FSMConstant;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FSM.Web.Areas.Customer.ViewModels
{
    public class CustomerConditionReportViewModel
    {
        [Key]
        public Guid ConditionReportId { get; set; }
        public Guid SiteDetailId { get; set; }
        [DisplayName("Roof Type")]
        public Constant.RoofType RoofTilesSheets { get; set; }
        public string DisplayRoofTilesSheet { get; set; }
        [DisplayName("Barge Cappings")]
        public Constant.BargeCappings BargeCappings { get; set; }
        [DisplayName("Ridge Cappings")]
        public Constant.RidgeCappings RidgeCappings { get; set; }
        public Constant.Valleys Valleys { get; set; }
        public string DisplayValley { get; set; }
        public Constant.flashings Flashings { get; set; }
        public string DisplayFlashing { get; set; }
        public Constant.GutarGuard Gutters { get; set; }
        public string DisplayGutter { get; set; }
        [DisplayName("Down Pipes")]
        public Constant.DownPipe DownPipes { get; set; }
        [DisplayName("Notes")]
        public string ConditionNote { get; set; }
        [DisplayName("Condition Roof")]
        public Constant.ConditionRoof ConditionRoof { get; set; }
        public string DisplayDownPipe { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
    }
}