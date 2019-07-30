using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static FSM.Web.FSMConstant.Constant;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class ContractJobScheduleViewModel
    {
        public Guid JobId { get; set; }
        [Display(Name = "Next Contract Schedule Date")]
        public Nullable<DateTime> NextContractScheduleDate { get; set; }
        [Display(Name = "Notification Type")]
        public JobNotificationType JobNotificationType { get; set; }
    }
}