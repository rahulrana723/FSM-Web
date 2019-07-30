using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static FSM.Web.FSMConstant.Constant;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class CancelJobScheduleViewModel
    {
        public Guid JobId { get; set; }
        [DisplayName("SRAS Needed")]
        public bool SRASNeeded { get; set; }
        [DisplayName("Price Increase Approval Needed")]
        public bool PriceApprovalNeeded { get; set; }
        [DisplayName("Deselect Contract")]
        public bool DeselectContract { get; set; }
        public string Reason { get; set; }
    }
}